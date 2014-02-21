namespace ExpressionKit
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  /// <summary>
  /// Used to declare that this method is unwrappable by an expression visitor.
  /// This attribute provides no additional functionality.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  internal class UnwrappableMethodAttribute : Attribute { }
}
