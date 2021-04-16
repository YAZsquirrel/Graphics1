using SharpGL;
using SharpGL.WPF;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Graphics1
{
   public partial class MainWindow : Window
   {
      enum ColorName
      { Red, Green, Blue }
      public MainWindow()
      {
         InitializeComponent();
      }
      struct Point
      {
         public double X { get; set; }
         public double Y { get; set; }
         public double[] Color { get; set; }
         public Point(double _x, double _y, double[] rgba)
         {
            X = _x; Y = _y;
            Color = rgba;
         }
         public void SetColor(double[] col)
         {
            Color = col;
         }
         public void SetPosition(double[] xy)
         {
            X = xy[0]; Y = xy[1];
         }
         public void SetPosition(double x, double y)
         {
            X = x; Y = y;
         }
         public void SetColor(double col, ColorName cn)
         {
            Color[(byte)cn] = col;
         }

      };
      struct Polygon
      {
         public List<Point?> Points { get; set; }
         public Polygon(List<Point?> points)
         {
            Points = points;
         }
      }

      List<List<Polygon?>> Sets = new List<List<Polygon?>>();

      Polygon? curPolygon;                               //To change and remove
      List<Point?> curPoints = new List<Point?>();       //
      Point? curPoint;
      List<Polygon?> curSet = new List<Polygon?>();

      Dictionary<string, List<Polygon?>> PolygonSetDict = new Dictionary<string, List<Polygon?>> { };

      double[] rgb = new double[3] { 1f, 1f, 1f };
      bool isCtrlDown = false;
      bool pointChanged = false;
      bool isSelected = false;
      bool isRMB = false;
      private void OpenGLControl_OpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
      {
         var gl = args.OpenGL;
         curPolygon = new Polygon(curPoints);
         curSet.Add(curPolygon);
         Sets.Add(curSet);
         gl.LineWidth(3);
      }
      private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
      {
         var gl = args.OpenGL;
         gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
         gl.ClearColor(1f, 1f, 1f, 1f);

         //Drawing current points
         
         gl.Enable(OpenGL.GL_POINT_SMOOTH);
         gl.Enable(OpenGL.GL_LINE_SMOOTH);

         foreach (var set in Sets)
         {
            foreach (var poly in set)
            {
               gl.Begin(OpenGL.GL_TRIANGLE_STRIP);
               foreach (var point in ((Polygon)poly).Points)
               {
                  gl.Color(((Point)point).Color);
                  gl.Vertex(((Point)point).X, ((Point)point).Y);
               }
               gl.End();
            }
         }

         foreach (var poly in curSet)
         {
            if (!poly.Equals(curPolygon)) // точками выделены текущий набор
            {
               gl.PointSize(7);
               gl.Begin(OpenGL.GL_POINTS);
               foreach (var point in ((Polygon)poly).Points)
               {
                  gl.Color(1f - ((Point)point).Color[0], 1f - ((Point)point).Color[1], 1f - ((Point)point).Color[2]);
                  gl.Vertex(((Point)point).X, ((Point)point).Y);
               }
               gl.End();
            }
            else
            {
               foreach (var point in ((Polygon)curPolygon).Points)
               {
                  if (point.Equals(curPoint))
                     gl.PointSize(15);
                  else
                     gl.PointSize(7);
                  gl.Begin(OpenGL.GL_POINTS);
                  gl.Color(1f - ((Point)point).Color[0], 1f - ((Point)point).Color[1], 1f - ((Point)point).Color[2]);
                  gl.Vertex(((Point)point).X, ((Point)point).Y);
                  gl.End();
               }

               gl.Begin(OpenGL.GL_LINE_LOOP);   // линиями выделен текущий полигон
               foreach (var point in ((Polygon)curPolygon).Points)
               {
                  gl.Color(1f - ((Point)point).Color[0], 1f - ((Point)point).Color[1], 1f - ((Point)point).Color[2]);
                  gl.Vertex(((Point)point).X, ((Point)point).Y);
               }
               gl.End();
            }
         }

         gl.Finish();
         if (curPoint != null) Title = $"Лабораторная работа 1 {{ {((Point)curPoint).X / 2 + 0.5} , {((Point)curPoint).Y / 2 - 0.5} }} ({rgb[0] * 255}, {rgb[1] * 255}, {rgb[2] * 255})";
         else Title = $"Лабораторная работа 1";
      }
      private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
      {
      }
      #region Color Management
      private void OGLControl_ColorPicked_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
      {
         var gl = args.OpenGL;
         gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
         gl.ClearColor((float)rgb[0], (float)rgb[1], (float)rgb[2], 0);
      }

      private void Slider_Red_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
      {
         TextBox_R.Text = $"{Math.Floor(Slider_Red.Value)}";
         rgb[0] = Slider_Red.Value / 255;
         if (curPoint != null && !pointChanged) { ((Point)curPoint).SetColor(rgb[0], ColorName.Red); pointChanged = false; }
      }
      private void Slider_Green_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
      {
         TextBox_G.Text = $"{Math.Floor(Slider_Green.Value)}";
         rgb[1] = Slider_Green.Value / 255;
         if (curPoint != null && !pointChanged) { ((Point)curPoint).SetColor(rgb[1], ColorName.Green); pointChanged = false; }
      }
      private void Slider_Blue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
      {
         TextBox_B.Text = $"{Math.Floor(Slider_Blue.Value)}";
         rgb[2] = Slider_Blue.Value / 255;
         if (curPoint != null && !pointChanged) { ((Point)curPoint).SetColor(rgb[2], ColorName.Blue); pointChanged = false; }
      }
      private void TextBox_R_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
      {
         try
         {
            if (Convert.ToInt32(TextBox_R.Text) < 0) { Slider_Red.Value = 0; TextBox_R.Text = "0"; }
            else if (Convert.ToInt32(TextBox_R.Text) > 255) { Slider_Red.Value = 255; TextBox_R.Text = "255"; }
            else Slider_Red.Value = Convert.ToInt32(TextBox_R.Text);
         }
         catch
         {
            Slider_Red.Value = 0;
            TextBox_R.Text = "0";
         }
         if (!pointChanged) rgb[0] = Slider_Red.Value / 255;
      }
      private void TextBox_G_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
      {
         try
         {
            if (Convert.ToInt32(TextBox_G.Text) < 0) { Slider_Green.Value = 0; TextBox_G.Text = "0"; }
            else if (Convert.ToInt32(TextBox_G.Text) > 255) { Slider_Green.Value = 255; TextBox_G.Text = "255"; }
            else Slider_Green.Value = Convert.ToInt32(TextBox_G.Text);
         }
         catch
         {
            Slider_Green.Value = 0;
            TextBox_G.Text = "0";
         }
         if (!pointChanged) rgb[1] = Slider_Green.Value / 255;
      }
      private void TextBox_B_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
      {
         try
         {
            if (Convert.ToInt32(TextBox_B.Text) < 0) { Slider_Blue.Value = 0; TextBox_B.Text = "0"; }
            else if (Convert.ToInt32(TextBox_B.Text) > 255) { Slider_Blue.Value = 255; TextBox_B.Text = "255"; }
            else Slider_Blue.Value = Convert.ToInt32(TextBox_B.Text);
         }
         catch
         {
            Slider_Blue.Value = 0;
            TextBox_B.Text = "0";
         }
         if (!pointChanged) rgb[2] = Slider_Blue.Value / 255;
      }
      #endregion

      #region Controls
      private void OpenGLControl_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         var x = 2 * (e.GetPosition(sender as OpenGLControl).X / OpenGLControl.ActualWidth) - 1;
         var y = 1 - 2 * e.GetPosition(sender as OpenGLControl).Y / OpenGLControl.ActualHeight;

         if (!isCtrlDown)
         {
            curPoint = new Point(x, y, rgb.Clone() as double[]);
            curPoints.Add(curPoint);
         }
         else if (isCtrlDown && curSet.Count > 1 || (curSet.Count == 1 && ((Polygon)curSet[0]).Points.Count > 2))
         {
            int i = curPoints.IndexOf(curPoint);
            curPoints[i] = curPoint = new Point(x, y, ((Point)curPoint).Color);
         }
      }
      private void OpenGLControl_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
      {
         if (curPolygon != null && Sets[0].Count > 0)
         {
            isRMB = true;
            var x = 2 * (e.GetPosition(sender as OpenGLControl).X / OpenGLControl.ActualWidth) - 1;
            var y = 1 - 2 * e.GetPosition(sender as OpenGLControl).Y / OpenGLControl.ActualHeight;
            double min;
            double minp = double.MaxValue;
            bool foundLesser;
            foreach (var set in Sets)
            {
               foreach (Polygon? poly in set)
               {
                  foundLesser = false;
                  foreach (Point? point in ((Polygon)poly).Points)
                  {
                     min = Math.Min(Math.Sqrt((x - ((Point)point).X) * (x - ((Point)point).X)
                                            + (y - ((Point)point).Y) * (y - ((Point)point).Y)), minp);
                     if (min != minp)
                     {
                        curPoint = point; foundLesser = true; minp = min;
                     }
                  }
                  if (curPoint != null && ((Polygon)poly).Points.Count > 0 && foundLesser)
                  { curPolygon = poly; curPoints = ((Polygon)poly).Points; }
               }
               if (curPoint != null && set.Contains(curPolygon)) curSet = set;
            }

            rgb = ((Point)curPoint).Color.Clone() as double[];

            Slider_Red.Value = rgb[0] * 255;
            Slider_Green.Value = rgb[1] * 255;
            Slider_Blue.Value = rgb[2] * 255;
            pointChanged = false;

            if (PolygonSetDict.ContainsValue(curSet))
            {
               string Key = "";
               foreach (var key in PolygonSetDict.Keys)
               {
                  Key = PolygonSetDict[key] == curSet ? key : "";
               }
               ListBox_PolygonList.SelectedIndex = ListBox_PolygonList.Items.IndexOf(Key);
            }
         }
      }
      private void Button_DeleteAll_Click(object sender, RoutedEventArgs e)
      {
         curPoints.Clear();
         if (curSet != null && Sets.Count > 1)
         {
            Sets.Remove(curSet);
            if (ListBox_PolygonList.SelectedIndex >= 0)
            {
               PolygonSetDict.Remove((string)ListBox_PolygonList.SelectedItem);
               ListBox_PolygonList.Items.Remove(ListBox_PolygonList.SelectedItem);
            }
            curSet.Clear();

            curPoints = new List<Point?>();
            curPoint = null;
            curSet = new List<Polygon?>();
            curPolygon = new Polygon(curPoints);
            curSet.Add(curPolygon);
            Sets.Add(curSet);
         }
         else if (Sets.Count == 1 || (Sets.Count == 2 && Sets[1].Count != 0))
         {
            Sets[0].Clear();

            curPoints = new List<Point?>();
            curPoint = null;
            curSet = new List<Polygon?>();
            curPolygon = new Polygon(curPoints);
            curSet.Add(curPolygon);
            Sets[0] = curSet;
         }

      }
      private void Button_NewPolygon_Click(object sender, RoutedEventArgs e) // Add new polygon to cur set
      {
         if (curPoints.Count > 2 && (ListBox_PolygonList.SelectedIndex == -1 || isSelected))
         { 
            curPoint = null;
            curPoints = new List<Point?>();
            curSet.Add(curPolygon = new Polygon(curPoints));
         }
      }
      private void Button_DeleteLast_Click(object sender, RoutedEventArgs e) // delete cur point
      {
         if (curPoints.Count > 1)
         {
            curPoints.RemoveAt(curPoints.IndexOf(curPoint));
            if (curPoints.Count > 1)
               curPoint = curPoints[^1];
            else
            {
               if (curSet.Count > 1)
                  curSet.RemoveAt(curSet.IndexOf(curPolygon));
               else
               {
                  if (Sets.Count > 1) Sets.RemoveAt(Sets.IndexOf(curSet));
                  else ((Polygon)curSet[0]).Points.Clear();
               }
               curPoint = null;
            }
         }
      }
      private void MyWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
      {
         isCtrlDown = (e.Key == System.Windows.Input.Key.LeftCtrl || e.Key == System.Windows.Input.Key.RightCtrl) && MyWindow.IsActive;
      }
      private void MyWindow_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
      {
         isCtrlDown = !(e.Key == System.Windows.Input.Key.LeftCtrl || e.Key == System.Windows.Input.Key.RightCtrl);
      }
      private void Button_AddNew_Click(object sender, RoutedEventArgs e)      //add new set
      {
         if (curPoints.Count > 2 && (ListBox_PolygonList.SelectedIndex == -1 || isSelected))
            curSet.Add(curPolygon = new Polygon(new List<Point?>(curPoints)));
         if (curSet.Count > 1 || (curSet.Count == 1 && ((Polygon)curSet[0]).Points.Count > 2)) // новые полигоны
         {      
            if ((ListBox_PolygonList.SelectedIndex == -1 && !PolygonSetDict.ContainsValue(curSet)) || isSelected) //если выбора нет(новый) или сохраняем, что не сохранено
            {
               while (PolygonSetDict.ContainsKey(TextBox_PolygonName.Text))
                  TextBox_PolygonName.Text += "_Copy";
               PolygonSetDict.Add(TextBox_PolygonName.Text, curSet);
               ListBox_PolygonList.Items.Add(TextBox_PolygonName.Text);

               if (!isSelected)
               {
                  curSet = new List<Polygon?>();
                  Sets.Add(curSet);
                  curPoint = null;
                  curPoints = new List<Point?>();
                  curPolygon = new Polygon(curPoints);
                  curSet.Add(curPolygon);
               }
            }
            else if (!isSelected && !isRMB) //если нажал кнопку и есть выбор(обновляем)
            {
               PolygonSetDict.Remove(ListBox_PolygonList.SelectedItem.ToString());              
               ListBox_PolygonList.Items.Remove(ListBox_PolygonList.SelectedItem);            
               PolygonSetDict.Add(TextBox_PolygonName.Text, curSet);
               ListBox_PolygonList.Items.Add(TextBox_PolygonName.Text);
               ListBox_PolygonList.UnselectAll();

               curSet = new List<Polygon?>();
               Sets.Add(curSet);
               curPoint = null;
               curPoints = new List<Point?>();
               curPolygon = new Polygon(curPoints);
               curSet.Add(curPolygon);
            }
         }
         else if (curSet.Count == 1 && ((Polygon)curSet[0]).Points.Count < 3 && !isSelected) // новых полигонов нет
         {
            curSet = new List<Polygon?>();
            curPoint = null;
            curPoints = new List<Point?>();
            curPolygon = new Polygon(curPoints);
            curSet.Add(curPolygon);
         }
      }
      #endregion
      private void ListBox_PolygonList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
      {
         if (isSelected = ListBox_PolygonList.SelectedIndex != -1)
         {
            isSelected = !isRMB;
            Button_AddNew_Click(sender, e);
            isSelected = false;
            isRMB = false;
            TextBox_PolygonName.Text = ListBox_PolygonList.SelectedItem.ToString();
            curSet = PolygonSetDict[ListBox_PolygonList.SelectedItem.ToString()];
            curPolygon = curSet[0];
            curPoint = ((Polygon)curPolygon).Points[0];
            curPoints = ((Polygon)curPolygon).Points;

            rgb = ((Point)curPoint).Color.Clone() as double[];

            Slider_Red.Value = rgb[0] * 255;
            Slider_Green.Value = rgb[1] * 255;
            Slider_Blue.Value = rgb[2] * 255;
            pointChanged = false;
         }
      }
   }
}
