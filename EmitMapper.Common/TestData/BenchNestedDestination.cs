namespace EmitMapper.Common.TestData;

public class BenchNestedDestination : ITestObject
{
  public long N2;
  public long N3;
  public long N4;
  public long N5;
  public long N6;
  public long N7;
  public long N8;
  public long N9;

  public string S1;
  public string S2;
  public string S3;
  public string S4;
  public string S5;
  public string S6;
  public string S7;

  public Inner1 I1 { get; set; }
  public Inner1 I2 { get; set; }
  public Inner1 I3 { get; set; }
  public Inner1 I4 { get; set; }
  public Inner1 I5 { get; set; }
  public Inner1 I6 { get; set; }
  public Inner1 I7 { get; set; }
  public Inner1 I8 { get; set; }


  public class Inner1
  {
    public Inner2 I1;
    public Inner2 I2;
    public Inner2 I3;
    public Inner2 I4;
    public Inner2 I5;
    public Inner2 I6;
    public Inner2 I7;
  }

  public class Inner2
  {
    public int I;
    public string Str1;
    public string Str2;
  }
}