using System;
using EmitMapper.Utils;
using Xunit;

namespace EmitMapper.Tests
{
  public class TestObject
  {
    public int SomeField = 5;
    public readonly int SomeReadonlyField = 55;
    public const string SomeConstField = "This is a const field";

    public int SomeProperty { get; set; }
    public int SomeBackedProperty { get { return SomeField; } set { SomeField = value; } }
    public int SomeReadonlyProperty { get { return SomeReadonlyField; } }
    public string SomeConstProperty { get { return SomeConstField; } }
  }


  public class FastReflectionTests
  {
    [Fact]
    public void Can_Get_Property_Getter()
    {
      var propertyInfo = typeof(TestObject).GetProperty("SomeProperty");
      MemberGetter<object, object> getter = null;

      getter = propertyInfo.DelegateForGet();
      Assert.NotNull(getter);
    }

    [Fact]
    public void Can_Get_Property()
    {
      var testObject = new TestObject();
      var propertyInfo = typeof(TestObject).GetProperty("SomeProperty");
      var getter = propertyInfo.DelegateForGet();

      Assert.Equal(0, getter(testObject));
    }

    [Fact]
    public void Can_Get_Property_Setter()
    {
      var propertyInfo = typeof(TestObject).GetProperty("SomeProperty");
      MemberSetter<object, object> setter = null;
      setter = propertyInfo.DelegateForSet();
      Assert.NotNull(setter);
    }

    [Fact]
    public void Can_Set_Property()
    {
      var testObject = new TestObject();
      var testObjectAsObj = (object)testObject;
      var propertyInfo = typeof(TestObject).GetProperty("SomeProperty");
      var setter = propertyInfo.DelegateForSet();
      const int valueToSet = 123;
      setter(ref testObjectAsObj, valueToSet);
      Assert.Equal(valueToSet, testObject.SomeProperty);
    }

    [Fact]
    public void Can_Get_Field_Getter()
    {
      var fieldInfo = typeof(TestObject).GetField("SomeField");
      MemberGetter<object, object> getter = null;

      getter = fieldInfo.DelegateForGet();
      Assert.NotNull(getter);
    }

    [Fact]
    public void Can_Get_Field()
    {
      var testObject = new TestObject();
      var fieldInfo = typeof(TestObject).GetField("SomeField");
      var getter = fieldInfo.DelegateForGet();

      Assert.Equal(5, getter(testObject));
    }

    [Fact]
    public void Can_Get_Field_Setter()
    {
      var fieldInfo = typeof(TestObject).GetField("SomeField");
      MemberSetter<object, object> setter = null;
      setter = fieldInfo.DelegateForSet();
      Assert.NotNull(setter);
    }

    [Fact]
    public void Can_Set_Field()
    {
      var testObject = new TestObject();
      var testObjectAsObj = (object)testObject;
      var fieldInfo = typeof(TestObject).GetField("SomeField");
      var setter = fieldInfo.DelegateForSet();
      const int valueToSet = 123;
      setter(ref testObjectAsObj, valueToSet);
      Assert.Equal(valueToSet, testObject.SomeField);
    }

    [Fact]
    public void Can_Get_Readonly_Field_Getter()
    {
      var fieldInfo = typeof(TestObject).GetField("SomeReadonlyField");
      MemberGetter<object, object> getter = null;
      getter = fieldInfo.DelegateForGet();
      Assert.NotNull(getter);
    }

    [Fact]
    public void Can_Get_Readonly_Field()
    {
      var testObject = new TestObject();
      var fieldInfo = typeof(TestObject).GetField("SomeReadonlyField");
      var getter = fieldInfo.DelegateForGet();

      Assert.Equal(55, getter(testObject));
    }

    [Fact]
    public void Can_Get_Const_Field_Getter()
    {
      var fieldInfo = typeof(TestObject).GetField("SomeConstField");
      MemberGetter<object, object> getter = null;
      getter = fieldInfo.DelegateForGet();
      Assert.NotNull(getter);
    }

    [Fact]
    public void Can_Get_Const_Field()
    {
      var testObject = new TestObject();
      var fieldInfo = typeof(TestObject).GetField("SomeConstField");
      var getter = fieldInfo.DelegateForGet();

      Assert.Equal("This is a const field", getter(testObject));
    }

    [Fact]
    public void Cant_Set_Const_Field()
    {
      var fieldInfo = typeof(TestObject).GetField("SomeConstField");

      Assert.Throws<NotSupportedException>(() => fieldInfo.DelegateForSet());
    }

    [Fact]
    public void Can_Get_Readonly_Backing_Field_Getter()
    {
      var propertyInfo = typeof(TestObject).GetProperty("SomeReadonlyProperty");
      MemberGetter<object, object> getter = null;
      getter = propertyInfo.DelegateForGet();
      Assert.NotNull(getter);
    }

    [Fact]
    public void Can_Get_Readonly_Backing_Field()
    {
      var testObject = new TestObject();
      var propertyInfo = typeof(TestObject).GetProperty("SomeReadonlyProperty");
      var getter = propertyInfo.DelegateForGet();

      Assert.Equal(55, getter(testObject));
    }

    [Fact]
    public void Can_Get_Const_Backing_Field_Getter()
    {
      var propertyInfo = typeof(TestObject).GetProperty("SomeConstProperty");
      MemberGetter<object, object> getter = null;
      getter = propertyInfo.DelegateForGet();
      Assert.NotNull(getter);
    }

    [Fact]
    public void Can_Get_Const_Backing_Field()
    {
      var testObject = new TestObject();
      var propertyInfo = typeof(TestObject).GetProperty("SomeConstProperty");
      var getter = propertyInfo.DelegateForGet();

      Assert.Equal("This is a const field", getter(testObject));
    }
  }
}