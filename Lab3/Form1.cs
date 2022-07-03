using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;


namespace Lab3
{
    public partial class Form1 : Form
    {
        int xn, yn, xk, yk;
        Bitmap mybitmap; 
        Color current_color = Color.Black;
        Color border = Color.Black;
        Stack<int> coordinateX = new Stack<int>();
        Stack<int> coordinateY = new Stack<int>();
        int i = 0;
        Bitmap finalImage;
        int[,] vershina = new int[100,2];
        int countVershina = 0;
        bool figura = true;
        bool polygonReady = false;
        bool polygonFill = false;

        /// <summary>
        /// Метод для объединения двух изображений
        /// </summary>
        /// <param name="overlayImage">Уже нанесенное на холст изображение</param>
        private void DrawImageToPictureBox(Bitmap overlayImage)
        {
            finalImage = new Bitmap(pictureBox1.Width, pictureBox1.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(finalImage);
            graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            graphics.DrawImage(mybitmap, 0, 0);
            graphics.DrawImage(overlayImage, 0, 0);
            pictureBox1.Image = finalImage;
        }

        /// <summary>
        /// ЦДА алгоритм для построения отрезков по координатам
        /// </summary>
        /// <param name="x1">Начало координат по Х</param>
        /// <param name="y1">Начало координат по У</param>
        /// <param name="x2">Конец координат по Х</param>
        /// <param name="y2">Конец координат по У</param>
        private void CDA(int x1, int y1, int x2, int y2)
        {
            if ((x1>500)||(x2>500)||(y1>500)||(y2>500)|| (x1 < 0) || (x2 < 0) || (y1 < 0) || (y2 < 0))
            {
                MessageBox.Show("Рисуйте мышью координаты лишь в пределах холста");
                return;
            }
            if (figura)
            {
                vershina[countVershina,0] = x1;
                vershina[countVershina, 1] = y1;
                countVershina = countVershina+1;
            }
            int i, n;
            double xt, yt, dx, dy;
            xn = x1;
            yn = y1;
            xk = x2;
            yk = y2;
            dx = xk - xn;
            dy = yk - yn;
            n = 500;
            xt = xn;
            yt = yn;
            for (i = 1; i <= n; i++)
            {
                mybitmap.SetPixel((int)xt, (int)yt, current_color);
                xt = xt + dx / n;
                yt = yt + dy / n;
            }
        }

        /// <summary>
        /// Рекурсиваная заливка с затравкой
        /// </summary>
        /// <param name="x1">Х координата затравки</param>
        /// <param name="y1">У координата затравки</param>
        private void RecursiveFill(int x1, int y1)
        {

            if ((y1 != 0) && (x1 != 0) && (y1 != 499) && (x1 != 499))
            {
                if ((mybitmap.GetPixel(x1, y1).ToArgb() != current_color.ToArgb())
                && (mybitmap.GetPixel(x1, y1).ToArgb() != border.ToArgb()))
                {
                    mybitmap.SetPixel(x1, y1, current_color);
                    RecursiveFill(x1 + 1, y1);
                    RecursiveFill(x1 - 1, y1);
                    RecursiveFill(x1, y1 + 1);
                    RecursiveFill(x1, y1 - 1);
                }
                else
                {
                    return;
                }
            } 
        }

        /// <summary>
        /// Итеративная заливка с затравкой
        /// </summary>
        /// <param name="x1">Х координата затравки</param>
        /// <param name="y1">У координата затравки</param>
        private void IterativeFill(int x1, int y1)
        {
            /*if (border.ToArgb()!=current_color.ToArgb())
            {
                MessageBox.Show("Граница фигуры должна быть одного цвета с ней");
                return;
            }*/
            //current_color = border;
            coordinateX.Push(x1);
            coordinateY.Push(y1);           
            while (coordinateY.Count!=0)
            {
                x1 = coordinateX.Pop();
                y1 = coordinateY.Pop();
                finalImage.SetPixel(x1, y1, current_color);
                if ((y1 != 0) && (x1 != 0)&& (y1 != 499) && (x1 != 499))
                {
                    if ((finalImage.GetPixel(x1 + 1, y1).ToArgb() != current_color.ToArgb()) 
                        && (finalImage.GetPixel(x1 + 1, y1).ToArgb() != border.ToArgb()))
                    {
                        coordinateX.Push(x1 + 1);
                        coordinateY.Push(y1);
                    }
                    if ((finalImage.GetPixel(x1 - 1, y1).ToArgb() != current_color.ToArgb()) 
                        && (finalImage.GetPixel(x1 - 1, y1).ToArgb() != border.ToArgb()))
                    {
                        coordinateX.Push(x1 - 1);
                        coordinateY.Push(y1);
                    }
                    if ((finalImage.GetPixel(x1, y1 + 1).ToArgb() != current_color.ToArgb()) 
                        && (finalImage.GetPixel(x1, y1 + 1).ToArgb() != border.ToArgb()))
                    {
                        coordinateX.Push(x1);
                        coordinateY.Push(y1 + 1);
                    }
                    if ((finalImage.GetPixel(x1, y1 - 1).ToArgb() != current_color.ToArgb()) 
                        && (finalImage.GetPixel(x1, y1 - 1).ToArgb() != border.ToArgb()))
                    {
                        coordinateX.Push(x1);
                        coordinateY.Push(y1 - 1);
                    }
                }
            }
        }

        /// <summary>
        /// Закраска контура фигуры
        /// </summary>
        private void ContourTraversal()
        {
            int x1 = 0;
            int y1 = 0;
            bool exit = false;
            while (!exit)
            {
                for (int x = 0; x < pictureBox1.Width; x++)
                {
                    for (int y = 0; y < pictureBox1.Height; y++)
                    {
                        if (finalImage.GetPixel(x, y).ToArgb() == border.ToArgb())
                        {
                            x1 = x;
                            y1 = y;
                            exit = true;
                        }
                    }
                }
            }
            coordinateX.Push(x1);
            coordinateY.Push(y1);
            while (coordinateY.Count != 0)
            {
                x1 = coordinateX.Pop();
                y1 = coordinateY.Pop();
                finalImage.SetPixel(x1, y1, current_color);

                if ((finalImage.GetPixel(x1 - 1, y1).ToArgb() != current_color.ToArgb())
                    && (finalImage.GetPixel(x1 - 1, y1).ToArgb() == border.ToArgb()))
                {
                    coordinateX.Push(x1 - 1);
                    coordinateY.Push(y1);
                }
                if ((finalImage.GetPixel(x1 - 1, y1 - 1).ToArgb() != current_color.ToArgb())
                && (finalImage.GetPixel(x1 - 1, y1 - 1).ToArgb() == border.ToArgb()))
                {
                    coordinateX.Push(x1 - 1);
                    coordinateY.Push(y1 - 1);
                }
                if ((finalImage.GetPixel(x1, y1 - 1).ToArgb() != current_color.ToArgb())
                    && (finalImage.GetPixel(x1, y1 - 1).ToArgb() == border.ToArgb()))
                {
                    coordinateX.Push(x1);
                    coordinateY.Push(y1 - 1);
                }
                if ((finalImage.GetPixel(x1 + 1, y1 - 1).ToArgb() != current_color.ToArgb())
                    && (finalImage.GetPixel(x1 + 1, y1 - 1).ToArgb() == border.ToArgb()))
                {
                    coordinateX.Push(x1 + 1);
                    coordinateY.Push(y1 - 1);
                }
                if ((finalImage.GetPixel(x1 + 1, y1).ToArgb() != current_color.ToArgb())
                    && (finalImage.GetPixel(x1 + 1, y1).ToArgb() == border.ToArgb()))
                {
                    coordinateX.Push(x1 + 1);
                    coordinateY.Push(y1);
                }
                if ((finalImage.GetPixel(x1 + 1, y1 + 1).ToArgb() != current_color.ToArgb())
                    && (finalImage.GetPixel(x1 + 1, y1 + 1).ToArgb() == border.ToArgb()))
                {
                    coordinateX.Push(x1 + 1);
                    coordinateY.Push(y1 + 1);
                }
                if ((finalImage.GetPixel(x1, y1 + 1).ToArgb() != current_color.ToArgb())
                && (finalImage.GetPixel(x1, y1 + 1).ToArgb() == border.ToArgb()))
                {
                    coordinateX.Push(x1);
                    coordinateY.Push(y1 + 1);
                }
                if ((finalImage.GetPixel(x1 - 1, y1 + 1).ToArgb() != current_color.ToArgb())
                && (finalImage.GetPixel(x1 - 1, y1 + 1).ToArgb() == border.ToArgb()))
                {
                    coordinateX.Push(x1 - 1);
                    coordinateY.Push(y1 + 1);
                }
            }
            border = current_color;
        }

        /// <summary>
        /// Алгоритм закраски многоугольника
        /// </summary>
        private void PolygonShading()
        {
            int yMin = 500;
            int yMax = 0;
            for (int x = 0; x < pictureBox1.Width; x++)
            {
                for (int y = 0; y < pictureBox1.Height; y++)
                {
                    if (finalImage.GetPixel(x, y).ToArgb() == border.ToArgb())
                    {
                        if (yMin >= y)
                        {
                            yMin = y;
                        }
                        if (yMax <= y)
                        {
                            yMax = y;
                        }
                    }
                }
            }
            int[] xj = new int[500];
            int pointAmount = 0;
            for (int y = yMin + 1; y < yMax - 3; y++)
            {
                for (int x = 1; x < pictureBox1.Width; x++)
                {
                    if ((finalImage.GetPixel(x, y).ToArgb() == border.ToArgb())
                        && (finalImage.GetPixel(x + 1, y).ToArgb() != border.ToArgb()))
                    {
                        xj[pointAmount] = x;
                        pointAmount++;
                    }
                }
                if ((pointAmount) % 2 != 0)
                {
                    int casd = pointAmount;
                    for (int k = 0; k < casd; k++)
                    {
                        for (int j = 0; j < countVershina; j++)
                        {
                            if ((xj[k] == vershina[j, 0]) && (y == vershina[j, 1]))
                            {
                                xj[pointAmount] = xj[k];
                                pointAmount++;
                            }
                        }
                    }
                }
                int count = 0;
                while (pointAmount > count)
                {
                    CDA(xj[count], y, xj[count + 1], y);
                    count = count + 2;
                }
                pointAmount = 0;
                Array.Clear(xj, 0, xj.Length);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            xn = e.X;
            yn = e.Y;

        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mybitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            if (radioButton1.Checked == true)
            {
                xk = e.X;
                yk = e.Y;
                CDA(xn, yn, xk, yk);
                border = current_color;
            }
            if (radioButton2.Checked == true)
            {
                mybitmap = pictureBox1.Image as Bitmap;
                IterativeFill(xn, yn);
            }
            if (radioButton3.Checked == true)
            {
                mybitmap = pictureBox1.Image as Bitmap;
                RecursiveFill(xn, yn);
            }
            if (i == 0)
            {
                i++;
                pictureBox1.Image = mybitmap;
                finalImage = mybitmap;
            }
            else
            {
                Bitmap overlayImage = (Bitmap)pictureBox1.Image;
                DrawImageToPictureBox(overlayImage);
            }
        }

        /// <summary>
        /// Выполнить построение заранее заданных фигур
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            mybitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            if (radioButton1.Checked == true)
            {
                border = current_color;
                CDA(10, 10, 10, 55);
                CDA(10, 55, 55, 55);
                CDA(55, 55, 55, 10);
                CDA(55, 10, 10, 10);
                CDA(150, 10, 150, 60);
                CDA(150, 60, 200, 35);
                CDA(200, 35, 150, 10);
                CDA(20, 120, 80, 140);
                CDA(80, 140, 35, 200);
                CDA(35, 200, 35, 175);
                CDA(35, 175, 10, 200);
                CDA(10, 200, 20, 120);
                if (i == 0)
                {
                    i++;
                    pictureBox1.Image = mybitmap;
                    finalImage = mybitmap;
                }
                else
                {
                    Bitmap overlayImage = (Bitmap)pictureBox1.Image;
                    DrawImageToPictureBox(overlayImage);
                }
            }
        }

        /// <summary>
        /// Выбрать цвет для отрезков и для заливки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult D = colorDialog1.ShowDialog();
            if (D == DialogResult.OK)
            {
                current_color = colorDialog1.Color;
                pictureBox2.BackColor = current_color;
            }
        }

        /// <summary>
        /// Очистить экран
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            polygonFill = false;
            polygonReady = false;
            i = 0;
            Array.Clear(vershina,0, countVershina);
            countVershina = 0;
            pictureBox1.Image = null;
            mybitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Bitmap overlayImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            DrawImageToPictureBox(overlayImage);
        }
        
        /// <summary>
        /// Построение многоугольника для дальнейшей его закраски
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            border = current_color;
            mybitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            figura = true;
            CDA(20, 20, 160, 80);
            CDA(160, 80, 70, 200);
            CDA(70, 200, 70, 150);
            CDA(70, 150, 20, 200);
            CDA(20, 200, 20, 20);
            if (i == 0)
            {
                i++;
                pictureBox1.Image = mybitmap;
                finalImage = mybitmap;
            }
            else
            {
                Bitmap overlayImage = (Bitmap)pictureBox1.Image;
                DrawImageToPictureBox(overlayImage);
            }
            figura = false;
            polygonReady = true;
            button4.Enabled = true;
        }

        /// <summary>
        /// Обход контура и закрашивание отрезками
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (!polygonReady)
            {
                MessageBox.Show("Сначала постройте фигуру, нажав на кнопку выше");
                return;
            }    
            if (polygonFill)
            {
                {
                    MessageBox.Show("Фигура уже закрашена");
                    return;
                }
            }
            button5.Enabled = false;
            mybitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            mybitmap = pictureBox1.Image as Bitmap;
            ContourTraversal();
            PolygonShading();
            if (i == 0)
            {
                i++;
                pictureBox1.Image = mybitmap;
                finalImage = mybitmap;
            }
            else
            {
                Bitmap overlayImage = (Bitmap)pictureBox1.Image;
                DrawImageToPictureBox(overlayImage);
            }
            polygonFill = true;
            button5.Enabled = true;
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false; 
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button4.Enabled = true;
            button5.Enabled = true;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
            mybitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Bitmap overlayImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            DrawImageToPictureBox(overlayImage);           
            if (i == 0)
            {
                i++;
                pictureBox1.Image = mybitmap;
                finalImage = mybitmap;
            }
            else
            {
                overlayImage = (Bitmap)pictureBox1.Image;
                DrawImageToPictureBox(overlayImage);
            }
            pictureBox2.BackColor = current_color;
        }

        public Form1()
        {
            InitializeComponent();
        }

        /*private void CohenSutherland()
        {
            int xMax = 335;
            int yMax = 335;
            int xMin = 165;
            int yMin = 165;
            int[,] razryad = new int[4, 3];//вверх низ право лево
            CDA(0, 0, 300, 300);
            int XN = 0;
            int YN = 0;
            int XK = 300;
            int YK = 300;
            if (YN < yMin)
            {
                razryad[0, 0] = 1;
            }
            else
            {
                razryad[0, 0] = 0;
            }
            if (YN > yMax)
            {
                razryad[1, 0] = 1;
            }
            else
            {
                razryad[1, 0] = 0;
            }
            if (XN > xMax)
            {
                razryad[2, 0] = 1;
            }
            else
            {
                razryad[2, 0] = 0;
            }
            if (XN < xMin)
            {
                razryad[3, 0] = 1;
            }
            else
            {
                razryad[3, 0] = 0;
            } 
            if (YK < yMin)
            {
                razryad[0, 1] = 1;
            }
            else
            {
                razryad[0, 1] = 0;
            }
            if (YK > yMax)
            {
                razryad[1, 1] = 1;
            }
            else
            {
                razryad[1, 1] = 0;
            }
            if (XK > xMax)
            {
                razryad[2, 1] = 1;
            }
            else
            {
                razryad[2, 1] = 0;
            }
            if (XK < xMin)
            {
                razryad[3, 1] = 1;
            }
            else
            {
                razryad[3, 1] = 0;
            }
            int sum = 0;
            int sum1 = 0;
            int sum2 = 0;
            int m = (YK - YN) / (XK - XN);

            for (int j = 0; j<4;j++)
            {
                razryad[j, 2] = razryad[j, 1] * razryad[j, 0];
                sum = sum + razryad[j, 2];
                sum1 = sum1 + razryad[j, 0];
                sum2 = sum2 + razryad[j, 1];
            }
            if (sum!=0)
            {
                MessageBox.Show("Отрезок вне окна");
            }
            else if ((sum1==0)&&(sum2==0))
            {
                MessageBox.Show("Отрезок внутри окна");
            }
            else
            {
                if(sum1!=0)
                {
                    if ((razryad[0, 0] == 1)  && (razryad[1, 0] == 0) && (razryad[2, 0] == 0) && (razryad[3, 0] == 1))
                    {
                        if ((razryad[2, 1] != 1) && (razryad[1, 1] == 1))
                        {
                            XN = XN + (1 / m) * (yMin - YN);//сверху
                            YN = yMin;
                        }
                        else if ((razryad[2, 1] == 1)&& (razryad[1, 1] != 1))
                        {
                            YN = m * (xMin - XN) + YN;//слева
                            XN = xMin;
                        }
                        else if ((razryad[2, 1] == 1) && (razryad[1, 1] == 1))
                        {
                            if (Math.Abs(m)>1)
                            {
                                YN = m * (xMin - XN) + YN;//слева
                                XN = xMin;
                            }
                            else if(Math.Abs(m) < 1)
                            {
                                XN = XN + (1 / m) * (yMin - YN);//сверху
                                YN = yMin;
                            }
                        }
                        else
                        {
                            XN = xMin;
                            YN = yMin;
                        }
                    }
                    if ((razryad[0, 0] == 1) && (razryad[1, 0] == 0) && (razryad[2, 0] == 0) && (razryad[3, 0] == 0))
                    {
                        if ((razryad[2, 1] != 1) && (razryad[1, 1] == 1))
                        {
                            XN = XN + (1 / m) * (yMin - YN);//сверху
                            YN = yMin;
                        }
                        else if ((razryad[2, 1] == 1) && (razryad[1, 1] != 1))
                        {
                            YN = m * (xMin - XN) + YN;//слева
                            XN = xMin;
                        }
                        else if ((razryad[2, 1] == 1) && (razryad[1, 1] == 1))
                        {
                            if (Math.Abs(m) > 1)
                            {
                                YN = m * (xMin - XN) + YN;//слева
                                XN = xMin;
                            }
                            else if (Math.Abs(m) < 1)
                            {
                                XN = XN + (1 / m) * (yMin - YN);//сверху
                                YN = yMin;
                            }
                        }
                        else
                        {
                            XN = xMin;
                            YN = yMin;
                        }

                    }
                }
            }
        }
*/
    }

}


