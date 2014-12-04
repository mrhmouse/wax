namespace ExpressionKit.Unwrap
{
  using System.Linq.Expressions;
  using System.Reflection;

  /// <summary>
  /// An expression visitor that inverts all
  /// logic in an expression tree.
  /// </summary>
  internal class InvertVisitor : ExpressionVisitor
  {
    private static Expression Invert(Expression e)
    {
      return Expression.Equal(e, Expression.Constant(false));
    }

    public override Expression Visit(Expression e)
    {
      if (e is BinaryExpression)
      {
        var b = e as BinaryExpression;
        switch (b.NodeType)
        {
          case ExpressionType.AndAlso:
            return Expression.OrElse(
              new InvertVisitor().Visit(b.Left),
              new InvertVisitor().Visit(b.Right));

          case ExpressionType.OrElse:
            return Expression.AndAlso(
              new InvertVisitor().Visit(b.Left),
              new InvertVisitor().Visit(b.Right));

          case ExpressionType.Equal:
            return Expression.NotEqual(
              b.Left,
              b.Right);

          case ExpressionType.NotEqual:
            return Expression.Equal(
              b.Left,
              b.Right);

          case ExpressionType.GreaterThanOrEqual:
            return Expression.LessThan(
              b.Left,
              b.Right);

          case ExpressionType.GreaterThan:
            return Expression.LessThanOrEqual(
              b.Left,
              b.Right);

          case ExpressionType.LessThanOrEqual:
            return Expression.GreaterThan(
              b.Left,
              b.Right);

          case ExpressionType.LessThan:
            return Expression.GreaterThanOrEqual(
              b.Left,
              b.Right);
        }
      }
      else if (e is UnaryExpression)
      {
        var u = e as UnaryExpression;
        switch (u.NodeType)
        {
          case ExpressionType.IsTrue:
            return Invert(u.Operand);

          case ExpressionType.IsFalse:
          case ExpressionType.Not:
            return u.Operand;
        }
      }
      else if (e is MemberExpression)
      {
        var m = e as MemberExpression;
        var transform = false;
        var boolean = typeof(bool);

        if (m.Member is PropertyInfo)
        {
          var p = m.Member as PropertyInfo;
          transform = boolean == p.PropertyType;
        }
        else if (m.Member is FieldInfo)
        {
          var f = m.Member as FieldInfo;
          transform = boolean == f.FieldType;
        }

        if (transform) return Invert(e);
      }

      return e;
    }
  }
}
