namespace Clipper2Lib.Test
{
  public class TestUnionPolygons
  {

    public static IEnumerable<object[]> GetPolysToUnite()
    {
      string filename1 = @".\Data\Poly99.txt";
      string filename2 = @".\Data\SideStart0_SideEnd1_Simple0.txt";
      string filename3 = @".\Data\SideStart2_SideEnd0_Simple0.txt";

      var poly1 = ReadPoly(filename1);
      var poly2 = ReadPoly(filename2);
      var poly3 = ReadPoly(filename3);

      yield return new object[] { new List<PathD> { poly1, poly2, poly3 } };
    }


    public static PathD ReadPoly(string filename)
    {
      string? line;
      var points = new List<PointD>();

      using (var sr = new StreamReader(filename))
      {

        line = sr.ReadLine();
        int numPoints = Convert.ToInt32(line);
        while (!sr.EndOfStream)
        {
          line = sr.ReadLine();
          string[] coords = line!.Split(';');
          if (coords.Length != 2)
          {
            throw new Exception("Invalid file format");
          }
          double X = double.Parse(coords[0]);
          double Y = double.Parse(coords[1]);

          points.Add(new PointD(X, Y));
        }

        if (points.Count != numPoints)
        {
          throw new Exception("Invalid number of points");
        }
      }



      return new PathD(points);

    }


    public static void WritePoly(string filename, PathD poly)
    {
      using (var sw = new StreamWriter(filename))
      {
        sw.WriteLine(poly.Count);

        for (int i = 0; i < poly.Count; i++)
        {
          PointD point = poly[i];
          sw.WriteLine($"{point.x:F16}; {point.y:F16}");
        }
      };





    }

    [Theory]
    [MemberData(nameof(GetPolysToUnite))]
    public void TestUnion(IEnumerable<PathD> polys)
    {
      var clipper = new ClipperD(5);

      foreach (var poly in polys)
      {
        clipper.AddSubject(poly);

      }

      var polyTree = new PolyTreeD();
      bool ok = clipper.Execute(ClipType.Union, FillRule.Positive, polyTree);
      Assert.True(ok);
      Assert.Equal(1, polyTree.Count);

      WritePoly(@".\Data\resultPoly.txt", polyTree[0].Polygon!);

    }
  }
}