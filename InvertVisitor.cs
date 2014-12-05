namespace ExpressionKit.Unwrap
{
  using System;
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

    protected override Expression VisitBinary(BinaryExpression b)
    {
      switch (b.NodeType)
      {
        case ExpressionType.AndAlso:
          return Expression.OrElse(
            this.Visit(b.Left),
            this.Visit(b.Right));

        case ExpressionType.OrElse:
          return Expression.AndAlso(
            this.Visit(b.Left),
            this.Visit(b.Right));

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

        default:
          return b;
      }
    }

    protected override Expression VisitUnary(UnaryExpression u)
    {
      switch (u.NodeType)
      {
        case ExpressionType.IsTrue:
          return Invert(u.Operand);

        case ExpressionType.IsFalse:
        case ExpressionType.Not:
          return u.Operand;

        default:
          return u;
      }
    }

    protected override Expression VisitMember(MemberExpression m)
    {
      if (m.Type != typeof(bool))
        return base.VisitMember(m);

      return Invert(m);
    }

    protected override Expression VisitMethodCall(
      MethodCallExpression m)
    {
      if (m.Type != typeof(bool))
        return base.VisitMethodCall(m);

      return Invert(m);
    }

    protected override Expression VisitConditional(
      ConditionalExpression c)
    {
      return Expression.Condition(
        c.Test,
        c.IfFalse,
        c.IfTrue);
    }

    protected override Expression VisitConstant(
      ConstantExpression c)
    {
      if (c.Type != typeof(bool))
        return base.VisitConstant(c);

      var val = (bool)c.Value;
      return Expression.Constant(!val);
    }

    protected override Expression VisitDefault(
      DefaultExpression d)
    {
      if (d.Type != typeof(bool))
        return base.VisitDefault(d);

      return Expression.Constant(true);
    }

    protected override Expression VisitDynamic(
      DynamicExpression d)
    {
      if (d.Type != typeof(bool))
        return base.VisitDynamic(d);

      return Invert(d);
    }
  }
}
