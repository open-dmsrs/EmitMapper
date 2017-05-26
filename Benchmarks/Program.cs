using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Benchmarks
{
	public class Program
	{
		static void Main(string[] args)
		{
			SimpleTest.Initialize();
			NestedClassesTest.Initialize();
			CustomizationTest.Initialize();

			SimpleTest.Run();
			NestedClassesTest.Run();
			CustomizationTest.Run();
		}
	}
}

/*
Auto Mapper (simple): 38483 milliseconds
BLToolkit (simple): 37019 milliseconds
Emit Mapper (simple): 118 milliseconds
Handwritten Mapper (simple): 37 milliseconds
Auto Mapper (Nested): 53800 milliseconds
Emit Mapper (Nested): 130 milliseconds
Handwritten Mapper (Nested): 128 milliseconds
Auto Mapper (Custom): 49587 milliseconds
Emit Mapper (Custom): 231 milliseconds
*/