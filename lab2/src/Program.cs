using System;


namespace lab2
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1.
            /*
            V4DataArray dataArray = new V4DataArray("data array", new DateTime(2015, 7, 20), 2, 3, new Vec2(0.5f, 0.75f), Fv2Methods.LinearFunc);
            Console.WriteLine(V4DataArray.SaveBinary("1", dataArray));
            Console.WriteLine(dataArray.ObjectType);
            Console.WriteLine(dataArray.LastChangeDate);
            foreach (var x in dataArray)
            {
                Console.WriteLine(x);
            }

            V4DataArray a = new V4DataArray("data array", new DateTime());
            Console.WriteLine(V4DataArray.LoadBinary("1", ref a));
            Console.WriteLine(dataArray.ObjectType);
            Console.WriteLine(a.LastChangeDate);
            foreach (var x in a)
            {
                Console.WriteLine(x);
            }
            */
            /*Console.WriteLine(dataArray.ToLongString("f4"));

            V4DataList dataList = (V4DataList) dataArray;
            Console.WriteLine(dataList.ToLongString("f3"));

            Console.WriteLine($"dataArray: count - {dataArray.Count} ; max from origin - {dataArray.MaxFromOrigin}");
            Console.WriteLine($"dataList: count - {dataList.Count} ; max from origin - {dataList.MaxFromOrigin}");
            Console.WriteLine();
        
            // 2.
            V4MainCollection mainCollection = new V4MainCollection();
            V4DataArray dataArray1 = new V4DataArray("1", new DateTime(), 1, 4, new Vec2(0.6f, 0.8f), Fv2Methods.LinearFunc);
            V4DataArray dataArray2 = new V4DataArray("2", new DateTime(), 4, 2, new Vec2(0.5f, 0.75f), Fv2Methods.MultiFunc);
            V4DataList dataList1 = new V4DataList("3", new DateTime());
            */
            V4DataList dataList2 = new V4DataList("4", new DateTime(2349, 9, 2));
            dataList2.AddDefaults(5, Fv2Methods.MultiFunc);
            Console.WriteLine(dataList2.ObjectType);
            Console.WriteLine(dataList2.LastChangeDate);
            foreach (var x in dataList2)
            {
                Console.WriteLine(x);
            }
            Console.WriteLine(V4DataList.SaveAsText(".2", dataList2));

            V4DataList a = new V4DataList("0", new DateTime());
            Console.WriteLine(V4DataList.LoadAsText(".2", ref a));
            Console.WriteLine(a.ObjectType);
            Console.WriteLine(a.LastChangeDate);
            foreach (var x in a)
            {
                Console.WriteLine(x);
            }
            /*
            mainCollection.Add(dataArray1);
            mainCollection.Add(dataArray2);
            mainCollection.Add(dataList1);
            mainCollection.Add(dataList2);
            Console.WriteLine(mainCollection.ToLongString("f2"));

            // 3.
            for (int i = 0; i < mainCollection.Count; ++i)
            {
                Console.WriteLine($"count - {mainCollection[i].Count}, max from origin - {mainCollection[i].MaxFromOrigin}");
            }
            */
        }
    }
}
