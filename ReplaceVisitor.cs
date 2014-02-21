namespace ExpressionKit.Unwrap
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Linq.Expressions;
  using System.Web;

  /// <summary>
  /// An expression visitor that acts as a simple find
  /// and replace.
  /// </summary>
  internal class ReplaceVisitor : ExpressionVisitor
  {
    /// <summary>
    /// The expression to find.
    /// </summary>
    private readonly Expression Source;

    /// <summary>
    /// The replacement expression.
    /// </summary>
    private readonly Expression Replacement;

    /// <summary>
    /// Create a new replacement visitor.
    /// </summary>
    /// <param name="source">
    /// The expression to find.
    /// </param>
    /// <param name="replacement">
    /// The replacement expression.
    /// </param>
    public ReplaceVisitor(Expression source, Expression replacement)
    {
      this.Source = source;
      this.Replacement = replacement;
    }

    public override Expression Visit(Expression node)
    {
      if (this.Source == node) return this.Replacement;

      return base.Visit(node);
    }
  }
}
