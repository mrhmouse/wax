namespace ExpressionKit.Unwrap
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  /// <summary>
  /// Used to mark constant expressions within queries.
  /// This attribute provides no additional functionality.
  /// </summary>
  [AttributeUsage(AttributeTargets.Method)]
  internal class ConstantValueMethodAttribute : Attribute { }
}
