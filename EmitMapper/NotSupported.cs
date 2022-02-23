namespace EmitMapper;

/// <summary>Indicates the not supported expression combination</summary>
public enum NotSupported
{
  /// <summary>Multi-dimensional array initializer is not supported</summary>
  NewArrayInit_MultidimensionalArray,
  /// <summary>Quote is not supported</summary>
  Quote,
  /// <summary>Dynamic is not supported</summary>
  Dynamic,
  /// <summary>RuntimeVariables is not supported</summary>
  RuntimeVariables,
  /// <summary>MemberInit MemberBinding is not supported</summary>
  MemberInit_MemberBinding,
  /// <summary>MemberInit ListBinding is not supported</summary>
  MemberInit_ListBinding,
  /// <summary>Goto of the Return kind from the TryCatch is not supported</summary>
  Try_GotoReturnToTheFollowupLabel,
  /// <summary>Not supported assignment target</summary>
  Assign_Target,
  /// <summary> ExpressionType.TypeEqual is not supported </summary>
  TypeEqual
}