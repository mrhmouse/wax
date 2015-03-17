namespace ExpressionKit.Unwrap
{
  using System;
  using System.Linq;
  using System.Linq.Expressions;

  /// <summary>
  /// Extensions for runtime expansion of expressions.
  /// </summary>
  public static class Wax
  {
    /// <summary>
    /// Inject a constant directly into the query.
    /// </summary>
    /// <typeparam name="TValue">
    /// The type of the result.
    /// </typeparam>
    /// <param name="value">
    /// The value to inject. It must not rely on
    /// parameters local to the query.
    /// </param>
    /// <returns>
    /// If this method is executed outside of an expression,
    /// it will throw an instance of <see cref="NotImplementedException"/>.
    /// </returns>
    [ConstantValueMethod]
    public static TValue Constant<TValue>(this TValue value)
    {
      throw new NotImplementedException();
    }

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
    /// This method should not be used outside of an unwrapped expression.
    /// </remarks>
    [UnwrappableMethod]
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
    /// This method should not be used outside of an unwrapped expression.
    /// </remarks>
    [UnwrappableMethod]
    public static TResult Expand<TParam1, TParam2, TResult>(
      this Expression<Func<TParam1, TParam2, TResult>> expression,
      TParam1 param1,
      TParam2 param2)
    {
      throw new InvalidOperationException();
    }

    /// <summary>
    /// Unwraps any <see cref="Expand{TParameter, TResult}(Expression{Func{TParameter, TResult}}, TParameter)"/>
    /// calls
    /// within the given expression.
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
    /// Combines a call to <see cref="Unwrap{TParam, TResult}(Expression{Func{TParam, TResult}})"/>
    /// with a call to
    /// <see cref="System.Linq.Queryable.Select{TSource, TResult}(IQueryable{TSource}, Expression{Func{TSource, TResult}})"/>.
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
    /// <remarks>
    /// Can be useful for anonymously typed objects.
    /// </remarks>
    public static IQueryable<TResult> UnwrappedSelect<TSource, TResult>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, TResult>> expression)
    {
      return source.Select(expression.Unwrap());
    }

    /// <summary>
    /// Combines a call to <see cref="Unwrap{TParam, TResult}(Expression{Func{TParam, TResult}})"/>
    /// with a call to
    /// <see cref="System.Linq.Queryable.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, Boolean}})"/>.
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
    /// <remarks>
    /// Can be useful for anonymously typed objects.
    /// </remarks>
    public static IQueryable<TSource> UnwrappedWhere<TSource>(
      this IQueryable<TSource> source,
      Expression<Func<TSource, bool>> filter)
    {
      return source.Where(filter.Unwrap());
    }

    /// <summary>
    /// Unwraps any <see cref="Expand{TParameter, TResult}(Expression{Func{TParameter, TResult}}, TParameter)"/>
    /// calls
    /// within the given expression.
    /// </summary>
    /// <typeparam name="TParam1">
    /// The first parameter type.
    /// </typeparam>
    /// <typeparam name="TParam2">
    /// The second parameter type.
    /// </typeparam>
    /// <typeparam name="TValue">
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
    /// An expression that simply returns false.
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
    /// An expression that simply returns true.
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
      var head = expressions.First();
      var tail = expressions.Skip(1);

      foreach (var expression in tail)
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
      var head = expressions.First();
      var tail = expressions.Skip(1);

      foreach (var expression in tail)
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
        new InvertVisitor().Visit(predicate.Body),
        predicate.Parameters);
    }
  }
}
