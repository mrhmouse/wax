namespace ExpressionKit.Unwrap
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Reflection;

  /// <summary>
  /// Unwraps calls to <seealso cref="Wax.Expand"/>
  /// into the definition of the expression they call.
  /// </summary>
  internal class UnwrapVisitor : ExpressionVisitor
  {
    private static Type DeclaringType = typeof(Wax);

    private static Type UnwrappableType = typeof(UnwrappableMethodAttribute);

    private static bool MethodMatches(MethodInfo method)
    {
      return method.DeclaringType == DeclaringType
        && method.IsPublic
        && method.CustomAttributes
          .Any(attr => attr.AttributeType == UnwrappableType);
    }

    // A dictionary of parameters to replace.
    private Dictionary<ParameterExpression, Expression> Replacements;

    /// <summary>
    /// Unwraps calls to unwrappable methods
    /// into the definition of the expression they call.
    /// </summary>
    internal UnwrapVisitor()
    {
      this.Replacements = new Dictionary<ParameterExpression, Expression>();
    }

    private UnwrapVisitor(Dictionary<ParameterExpression, Expression> replacements)
    {
      this.Replacements = replacements;
    }

    // Replace a parameter if it's in our dictionary of replacements
    protected override Expression VisitParameter(ParameterExpression node)
    {
      if (this.Replacements.ContainsKey(node))
        return this.Replacements[node];
      else
        return base.VisitParameter(node);
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
      if (!MethodMatches(node.Method)) return base.VisitMethodCall(node);

      // The first argument of an unwrappable call
      // is the expression to unwrap into.
      var expression = (node.Arguments[0] as MemberExpression);

      if (expression == null) return base.VisitMethodCall(node);

      // The owning object that holds our method.
      object constant;

      // The value of the field of `constant` - our method body.
      LambdaExpression lambda;

      if (expression.Expression is ConstantExpression)
        constant = (expression.Expression as ConstantExpression).Value;
      else
        // This is a static member.
        constant = expression.Member.ReflectedType;

      // The field or property of `constant` that we want.
      var field = expression.Member as FieldInfo;
      var property = expression.Member as PropertyInfo;

      if (property != null)
        lambda = property.GetValue(constant) as LambdaExpression;
      else if (field != null)
        lambda = field.GetValue(constant) as LambdaExpression;
      else
        return base.VisitMethodCall(node);

      var replacements = new Dictionary<ParameterExpression, Expression>();

      // Unwrap each parameter of the lambda by replacing
      // it with the correspoding argument to the outer
      // expression (the method call)
      for (var i = 0; i < lambda.Parameters.Count; i++)
      {
        // Recursively unwrap the entire tree.
        var replacement = this.Visit(node.Arguments[i + 1]);

        replacements.Add(lambda.Parameters[i], replacement);
      }

      // Allow another visit to replace parameters defined here and
      // to recursively unwrap method calls.
      return new UnwrapVisitor(replacements).Visit(lambda.Body);
    }
  }
}
