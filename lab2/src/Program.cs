using System;


namespace lab2
{
    class Program
    {
        static void TestFileInputOutput()
        {
            static void WriteV4Data(V4Data a)
            {
                Console.WriteLine(a.ObjectType);
                Console.WriteLine(a.LastChangeDate);
                foreach (var x in a)
                {
                    Console.WriteLine(x);
                }
            }
            // V4DataArray
            V4DataArray a = new V4DataArray("data array", new DateTime(2015, 7, 20), 2, 2, new Vec2(0.5f, 0.75f), Fv2Methods.LinearFunc);
            Console.WriteLine("Saved V4DataArray:");
            WriteV4Data(a);
            Console.WriteLine();
            Console.WriteLine($"Correctly saved: {V4DataArray.SaveBinary(".1", a)}");

            V4DataArray b = new V4DataArray("???", new DateTime());
            Console.WriteLine($"Correctly loaded: {V4DataArray.LoadBinary(".1", ref b)}\n");
            Console.WriteLine("Loaded V4DataArray:");
            WriteV4Data(b);
            Console.WriteLine();

            Console.WriteLine("##############################\n");

            // V4DataList
            V4DataList c = new V4DataList("data list", new DateTime(2349, 9, 2));
            c.AddDefaults(3, Fv2Methods.MultiFunc);
            Console.WriteLine("Saved V4DataList:");
            WriteV4Data(c);
            Console.WriteLine();
            Console.WriteLine($"Correctly saved: {V4DataList.SaveAsText(".2", c)}");

            V4DataList d = new V4DataList("???", new DateTime());
            Console.WriteLine($"Correctly loaded: {V4DataList.LoadAsText(".2", ref d)}\n");
            Console.WriteLine("Loaded V4DataList:");
            WriteV4Data(d);
            Console.WriteLine();
        }

        static void TestMainCollectionProperties()
        {
            V4MainCollection mainCollection = new V4MainCollection();
            V4DataArray dataArray1 = new V4DataArray("data array 1", new DateTime(), 1, 4, new Vec2(0.6f, 0.8f), Fv2Methods.LinearFunc);
            V4DataArray dataArray2 = new V4DataArray("data array 2", new DateTime(), 3, 2, new Vec2(0.5f, 0.75f), Fv2Methods.MultiFunc);
            V4DataArray dataArray3 = new V4DataArray("data array 3 (empty)", new DateTime());
            V4DataList dataList1 = new V4DataList("data list 1", new DateTime());
            V4DataList dataList2 = new V4DataList("data list 2", new DateTime(2349, 9, 2));
            V4DataList dataList3 = new V4DataList("data list 3 (empty)", new DateTime());
            dataList1.Add(new DataItem(new Vec2(0, 0.8f), new Vec2(8, 9)));
            dataList2.Add(new DataItem(new Vec2(1f, 0), new Vec2(9, 0)));
            dataList2.AddDefaults(3, Fv2Methods.MultiFunc);

            mainCollection.Add(dataArray1);
            mainCollection.Add(dataArray2);
            mainCollection.Add(dataArray3);
            mainCollection.Add(dataList1);
            mainCollection.Add(dataList2);
            mainCollection.Add(dataList3);
            
            Console.Write(mainCollection.ToLongString("f2"));

            Console.WriteLine("##############################\n");
            Console.WriteLine($"Max abs value of electrical field: {mainCollection.GetMaxAbsFieldValue}");

            Console.WriteLine("\n##############################\n");
            Console.WriteLine("Values of electrical field in decreasing order by distance from (0, 0):");
            foreach (var x in mainCollection.GetValuesInDecreasingOrderByDistance)
            {
                Console.WriteLine(x);
            }

            Console.WriteLine("\n##############################\n");
            Console.WriteLine("Coordinates containing in V4DataArrays and not containing in V4DataLists:");
            foreach (var x in mainCollection.GetArrayAndListDifferenceCoordinates)
            {
                Console.WriteLine(x);
            }
        }

        static void Main(string[] args)
        {

            TestFileInputOutput();
            Console.WriteLine("------------------------------\n");
            TestMainCollectionProperties();
        }
    }
}
