```javascript
非正则
typeof(void)  -->   Metadata.Void

typeof(Nullable<>)   -> Metadata.Nullable1

typeof(Action<>)   Metadata.Action1

typeof(Action<,>)   Metadata.Action2

typeof(Func<>)  --->   Metadata.Func1

typeof(ExpressionCompiler.ArrayClosure)   --->  Metadata<ExpressionCompiler.ArrayClosure>.Type



正则

typeof\(((?!ILGeneratorHacks)(?!ExpressionCompiler)(?!CurryClosureActions)(?!CurryClosureFuncs)[\w\[\]?\.]+)\)      Metadata<$1>.Type
 
 
 typeof\(Func<([\w,\s]+)>\)   --->
 FuncMetadata<$1>.Type
 
 typeof\(Action<([\w,\s]+)>\)   --->
 ActionMetadata<$1>.Type



```

