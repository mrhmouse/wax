Wax
===

Wax grew out of my frustration with Linq-to-SQL's inability to handle `InvocationExpression`s,
so its main purpose is to allow common expressions to be saved and re-used instead of repeated
verbatim each time. Wax also contains a few other functions that I found useful when
working with expressions.

The Functions
=============

Unwrap
------

This is the heart of Wax. It's used to unwrap other expressions into their definitions for you,
so that Linq-to-SQL (or perhaps other frameworks that expect simple expressions) can digest them.

There are two variations of `Unwrap`: one for functions receiving a single argument,
and another for functions receiving two arguments. In practice, I haven't needed more than this,
but I may extend these to the full length offered by `Func<T...>` in the future.

Expressions are marked for unwrapping using `Expand`, which also has two variants.

Example:

```csharp
using System;
using System.Linq;
using System.Linq.Expressions;

static class MyProgram
{
  static Expression<Func<MyModel, IQueryable<MyProperty>>>
    ModelProperties = /* some complex selection */;
    
  static void Main()
  {
    var red = MyContext.MyModels.Where(Wax.Unwrap<MyModel, bool>(m => ModelProperties
      .Expand(m)
      .Any(p => p.Color == Colors.Red)));
  }
}
```

Of course, having to explicitly state the type parameters for `Unwrap` every time can be irritating, and
it is impossible when one of the type parameters refers to an anonymous type.
Which is why Wax also provides...

UnwrappedWhere
--------------

This function is just Linq's `Where` combined with `Unwrap` to give you the convenience of type inference.
Using `UnwrappedWhere`, our above example becomes:

```csharp
using System;
using System.Linq;
using System.Linq.Expressions;

static class MyProgram
{
  static Expression<Func<MyModel, IQueryable<MyProperty>>>
    ModelProperties = /* some complex selection */;
    
  static void Main()
  {
    var red = MyContext.MyModels.UnwrappedWhere(m => ModelProperties
      .Expand(m)
      .Any(p => p.Color == Colors.Red)));
  }
}
```

UnwrappedSelect
---------------

This function is similar to `UnwrappedWhere`. You can probably guess what it does.
