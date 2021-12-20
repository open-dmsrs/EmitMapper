using System;
using System.Data;
using System.Linq;
using EmitMapper;
using EmitMapper.MappingConfiguration;
using EmitMapper.MappingConfiguration.MappingOperations;
using EmitMapper.MappingConfiguration.MappingOperations.Interfaces;
using EmitMapper.Utils;
using Xunit;

namespace SamplesTests;

public class TestMappingToDataRow
{
    [Fact]
    public void MappingToDataRow_test()
    {
        // this is the mapper 
        var mapper = ObjectMapperManager.DefaultInstance.GetMapper<TestDto, DataRow>(new Map2DataRowConfig());

        // initialization of test DTO object
        var testDataObject = new TestDto { Field1 = "field1", Field2 = 10, Field3 = true };

        // Initializing of test table. Usual this table is read from database.
        var dt = new DataTable();
        dt.Columns.Add("field1", typeof(string));
        dt.Columns.Add("field2", typeof(int));
        dt.Columns.Add("field3", typeof(bool));
        dt.Rows.Add();
        var dr = dt.Rows[0];

        // Mapping test object to datarow
        mapper.Map(testDataObject, dr);

        // Check if object is correctly mapped
        Assert.Equal("field1", dr["field1"]);
        Assert.Equal(10, dr["field2"]);
        Assert.Equal(true, dr["field3"]);
    }

    public class Map2DataRowConfig : MapConfigBase<Map2DataRowConfig>
    {
        public override IMappingOperation[] GetMappingOperations(Type from, Type to)
        {
            var objectMembers = ReflectionUtils.GetPublicFieldsAndProperties(from);
            return FilterOperations(
                    from,
                    to,
                    objectMembers.Select(m => (IMappingOperation)new SrcReadOperation { Source = new MemberDescriptor(m), Setter = (obj, value, state) => { ((DataRow)obj)[m.Name] = value ?? DBNull.Value; } })
                )
                .ToArray();
        }
    }
    // Using: 

    // Test data object
    public class TestDto
    {
        public bool Field3 = true;
        public int Field2 = 10;
        public string Field1 = "field1";
    }
}