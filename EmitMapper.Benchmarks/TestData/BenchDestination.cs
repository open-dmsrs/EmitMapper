namespace EmitMapper.Benchmarks.TestData;

public class BenchDestination
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

  public Int2 I1 { get; set; }
  public Int2 I2 { get; set; }
  public Int2 I3 { get; set; }
  public Int2 I4 { get; set; }
  public Int2 I5 { get; set; }
  public Int2 I6 { get; set; }
  public Int2 I7 { get; set; }
  public Int2 I8 { get; set; }

  public class Int1
  {
    public int I;
    public string Str1;
    public string Str2;
  }

  public class Int2
  {
    public Int1 I1;
    public Int1 I2;
    public Int1 I3;
    public Int1 I4;
    public Int1 I5;
    public Int1 I6;
    public Int1 I7;
  }
}