Wax
===

Wax grew out of my frustration with Linq-to-SQL's inability to handle `InvocationExpression`s,
so its main purpose is to allow common expressions to be saved and re-used instead of repeated
verbatim each time. Wax also contains a few other functions that I found useful when
working with expressions.

Installation
============

To install Wax, run

```
Install-Package Wax
```

in your NuGet package manager console, or download the source and compile with

```sh
# Assuming Mono's C# compiler
mcs -o+ -t:library -out:Wax.dll *.cs
```

Then, in your code,

```csharp
using ExpressionKit.Unwrap;
```

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
using ExpressionKit.Unwrap;

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
using ExpressionKit.Unwrap;

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

Or, And, Inverse
----------------

These three functions provide basic boolean logic for working with expressions.
Their functionality doesn't extend far beyond what's offerred by `Expression`,
but they are provided as extension methods, which I find easier to read.

Any, All
--------

These two functions are just shorthand for combining lists of expressions with
`Or` or `And`, respectively.

Expand
------

This function doesn't do much by itself; it's only used to flag which expressions to
`Unwrap`. When actually evaluated, it will throw an `InvalidOperationException`.
