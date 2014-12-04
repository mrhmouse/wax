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
      Type t;
      if (m.Member is PropertyInfo)
      {
        var p = m.Member as PropertyInfo;
        t = p.PropertyType;
      }
      else if (m.Member is FieldInfo)
      {
        var f = m.Member as FieldInfo;
        t = f.FieldType;
      }
      else return m;

      if (t == typeof(bool)) return Invert(m);
      else return m;
    }
  }
}
