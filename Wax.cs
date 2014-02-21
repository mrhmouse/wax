namespace ExpressionKit
{
  using System;
  using System.Linq;
  using System.Linq.Expressions;

  public static class Wax
  {
    /// <summary>
    /// Invoke an expression with a single parameter.
    /// </summary>
    /// <typeparam name="TParameter">
    /// The type of the parameter.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The return type of the expression.
    /// </typeparam>
    /// <param name="expression">
    /// The expression to invoke.
    /// </param>
    /// <param name="parameter">
    /// The parameter to pass to the expression.
    /// </param>
    /// <returns>
    /// The result of the expression.
    /// </returns>
    /// <remarks>
    /// When used in an expression, this call can be unwrapped.
    /// When used outside of an expression, this call takes
    /// place immediately.
    /// </remarks>
    [UnwrappableMethodAttribute]
    public static TResult Expand<TParameter, TResult>(
      this Expression<Func<TParameter, TResult>> expression,
      TParameter parameter)
    {
      throw new InvalidOperationException();
    }

    /// <summary>
    /// Invoke an expression with two parameters.
    /// </summary>
    /// <typeparam name="TParam1">
    /// The first parameter type.
    /// </typeparam>
    /// <typeparam name="TParam2">
    /// The second parameter type.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The return type of the expression.
    /// </typeparam>
    /// <param name="expression">
    /// The expression to invoke.
    /// </param>
    /// <param name="param1">
    /// The first parameter.
    /// </param>
    /// <param name="param2">
    /// The second parameter.
    /// </param>
    /// <returns>
    /// The result of the expression.
    /// </returns>
    /// <remarks>
    /// When used in an expression, this call can be unwrapped.
    /// When used outside of an expression, this call takes
    /// place immediately.
    /// </remarks>
    [UnwrappableMethodAttribute]
    public static TResult Expand<TParam1, TParam2, TResult>(
      this Expression<Func<TParam1, TParam2, TResult>> expression,
      TParam1 param1,
      TParam2 param2)
    {
      throw new InvalidOperationException();
    }

    /// <summary>
    /// Unwraps any <see cref="MethodCallExpression"/>
    /// expressions that have <see cref="UnwrappableMethodAttribute"/>
    /// applied to them.
    /// </summary>
    /// <typeparam name="TParam">
    /// The first parameter type.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The return type of the expression.
    /// </typeparam>
    /// <param name="expression">
    /// The expression to unwrap.
    /// </param>
    /// <returns>
    /// The unwrapped expression.
    /// </returns>
    public static Expression<Func<TParam, TResult>> Unwrap<TParam, TResult>(
      this Expression<Func<TParam, TResult>> expression)
    {
      return new UnwrapVisitor()
        .Visit(expression)
        as Expression<Func<TParam, TResult>>;
    }

    /// <summary>
    /// Combines a call to <see cref="Unwrap"/> with a
    /// call to <see cref="Select"/>.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of the source of elements.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The type of the projected value.
    /// </typeparam>
    /// <param name="source">
    /// The source of elements.
    /// </param>
    /// <param name="expression">
    /// The projector.
    /// </param>
    /// <returns>
    /// A collection.
    /// </returns>
    public static IQueryable<TResult> UnwrappedSelect<TSource, TResult>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, TResult>> expression)
    {
      return source.Select(expression.Unwrap());
    }

    /// <summary>
    /// Combines a call to <see cref="Unwrap"/> with a
    /// call to <see cref="Where"/>.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type of the source of elements.
    /// </typeparam>
    /// <param name="source">
    /// The source of elements.
    /// </param>
    /// <param name="filter">
    /// The filter.
    /// </param>
    /// <returns>
    /// A collection.
    /// </returns>
    public static IQueryable<TSource> UnwrappedWhere<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> filter)
    {
      return source.Where(filter.Unwrap());
    }

    /// <summary>
    /// Unwraps any <see cref="MethodCallExpression"/>
    /// expressions that have <see cref="UnwrappableMethodAttribute"/>
    /// applied to them.
    /// </summary>
    /// <typeparam name="TParam1">
    /// The first parameter type.
    /// </typeparam>
    /// <typeparam name="TParam2">
    /// The second parameter type.
    /// </typeparam>
    /// <typeparam name="TResult">
    /// The return type of the expression.
    /// </typeparam>
    /// <param name="expression">
    /// The expression to unwrap.
    /// </param>
    /// <returns>
    /// The unwrapped expression.
    /// </returns>
    public static Expression<Func<TParam1, TParam2, TValue>> Unwrap<TParam1, TParam2, TValue>(
      this Expression<Func<TParam1, TParam2, TValue>> expression)
    {
      return new UnwrapVisitor()
        .Visit(expression)
        as Expression<Func<TParam1, TParam2, TValue>>;
    }

    /// <summary>
    /// Helper for creating an expression that simply
    /// returns false.
    /// </summary>
    /// <typeparam name="TParam">
    /// The type of the parameter.
    /// </typeparam>
    /// <returns>
    /// An expression.
    /// </returns>
    public static Expression<Func<TParam, bool>> False<TParam>()
    {
      return model => false;
    }

    /// <summary>
    /// Helper for creating an expression that simply
    /// returns true.
    /// </summary>
    /// <typeparam name="TParam">
    /// The type of the parameter.
    /// </typeparam>
    /// <returns>
    /// An expression.
    /// </returns>
    public static Expression<Func<TParam, bool>> True<TParam>()
    {
      return model => true;
    }

    /// <summary>
    /// Combine two predicates with boolean OR logic.
    /// </summary>
    /// <typeparam name="TParam">
    /// The type of parameter for each expression.
    /// </typeparam>
    /// <param name="expression">
    /// The first expression.
    /// </param>
    /// <param name="condition">
    /// The second expression, only evaluated if the
    /// first returns false.
    /// </param>
    /// <returns>
    /// An expression.
    /// </returns>
    public static Expression<Func<TParam, bool>> Or<TParam>(
      this Expression<Func<TParam, bool>> expression,
      Expression<Func<TParam, bool>> condition)
    {
      var replaced = new ReplaceVisitor(
        condition.Parameters[0],
        expression.Parameters[0])
        .Visit(condition.Body);

      return Expression.Lambda<Func<TParam, bool>>(
        Expression.OrElse(
          expression.Body,
          replaced),
        expression.Parameters);
    }


    /// <summary>
    /// Combine two predicates with boolean AND logic.
    /// </summary>
    /// <typeparam name="TParam">
    /// The type of parameter for each expression.
    /// </typeparam>
    /// <param name="expression">
    /// The first expression.
    /// </param>
    /// <param name="condition">
    /// The second expression, not evaluated if the
    /// first returns false.
    /// </param>
    /// <returns>
    /// An expression.
    /// </returns>
    public static Expression<Func<TParam, bool>> And<TParam>(
      this Expression<Func<TParam, bool>> expression,
      Expression<Func<TParam, bool>> condition)
    {
      var replaced = new ReplaceVisitor(
        condition.Parameters[0],
        expression.Parameters[0])
        .Visit(condition.Body);

      return Expression.Lambda<Func<TParam, bool>>(
        Expression.AndAlso(
          expression.Body,
          replaced),
        expression.Parameters);
    }

    /// <summary>
    /// Combine a series of expressions with boolean AND logic.
    /// </summary>
    /// <typeparam name="TParam">
    /// The type of parameter for every expression.
    /// </typeparam>
    /// <param name="expressions">
    /// The expressions.
    /// </param>
    /// <returns>
    /// An expression.
    /// </returns>
    public static Expression<Func<TParam, bool>> All<TParam>(
      params Expression<Func<TParam, bool>>[] expressions)
    {
      var head = True<TParam>();

      foreach (var expression in expressions)
        head = head.And(expression);

      return head;
    }

    /// <summary>
    /// Combine a series of expressions with boolean OR logic.
    /// </summary>
    /// <typeparam name="TParam">
    /// The type of parameter for every expression.
    /// </typeparam>
    /// <param name="expressions">
    /// The expressions.
    /// </param>
    /// <returns>
    /// An expression.
    /// </returns>
    public static Expression<Func<TParam, bool>> Any<TParam>(
      params Expression<Func<TParam, bool>>[] expressions)
    {
      var head = False<TParam>();

      foreach (var expression in expressions)
        head = head.Or(expression);

      return head;
    }

    /// <summary>
    /// Invert a boolean expression.
    /// </summary>
    /// <typeparam name="TParam">
    /// The type of parameter for the expression.
    /// </typeparam>
    /// <param name="predicate">
    /// The expression.
    /// </param>
    /// <returns>
    /// An expression.
    /// </returns>
    public static Expression<Func<TParam, bool>> Inverse<TParam>(
      this Expression<Func<TParam, bool>> predicate)
    {
      return Expression.Lambda<Func<TParam, bool>>(
        Expression.Condition(
          predicate.Body,
          Expression.Constant(false),
          Expression.Constant(true)),
        predicate.Parameters);
    }
  }
}
