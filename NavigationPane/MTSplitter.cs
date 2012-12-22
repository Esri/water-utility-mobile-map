/*
 | Version 10.1.1
 | Copyright 2012 Esri
 |
 | Licensed under the Apache License, Version 2.0 (the "License");
 | you may not use this file except in compliance with the License.
 | You may obtain a copy of the License at
 |
 |    http://www.apache.org/licenses/LICENSE-2.0
 |
 | Unless required by applicable law or agreed to in writing, software
 | distributed under the License is distributed on an "AS IS" BASIS,
 | WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 | See the License for the specific language governing permissions and
 | limitations under the License.
 */


/*
 * Proje	    : MT Ortak kütüphane
 *
 * Hazırlayan   : Muhammed ŞAHİN
 * eMail        : muhammed.sahin@gmail.com
 *
 * Açıklama	    : Splitter nesnesi içerisinde dock durumuna bakılarak
 *                ayıraç karakteri ve 3D görünüm veriliyor.  
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;

namespace MobileControls
{
    /// <summary>
    /// Splitter, üzerinde ayıraç resmi çizili
    /// </summary>
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(System.Windows.Forms.Splitter))]
    [Description("3D Splitter")]
    public class MTSplitter : Splitter
    {

        #region SplitterLightColor
        private Color splitterLightColor = ProfessionalColors.SeparatorLight;
        /// <summary>
        /// Get/Set, Splitter için açık renk
        /// </summary>
        [Browsable(true)]
        [Description("Splitter için açık renk")]
        [Category("MT Kontrol")]
        [DefaultValue(typeof(Color), "ProfessionalColors.SeparatorLight")]
        public Color SplitterLightColor
        {
            get { return splitterLightColor; }
            set
            {
                splitterLightColor = value;
                this.Invalidate();
            }
        }
        #endregion

        #region SplitterDarkColor
        private Color splitterDarkColor = ProfessionalColors.SeparatorDark;
        /// <summary>
        /// Get/Set, Splitter için koyu renk
        /// </summary>
        [Browsable(true)]
        [Description("Splitter için koyu renk")]
        [Category("MT Kontrol")]
        [DefaultValue(typeof(Color), "ProfessionalColors.SeparatorDark")]
        public Color SplitterDarkColor
        {
            get { return splitterDarkColor; }
            set
            {
                splitterDarkColor = value;
                this.Invalidate();
            }
        }
        #endregion

        #region SplitterBorderColor
        private Color splitterBorderColor = Color.Transparent;
        /// <summary>
        /// Get/Set, Splitter için koyu renk
        /// </summary>
        [Browsable(true)]
        [Description("Splitter dış çizgi rengi")]
        [Category("MT Kontrol")]
        [DefaultValue(typeof(Color), "ProfessionalColors.SeparatorDark")]
        public Color SplitterBorderColor
        {
            get { return splitterBorderColor; }
            set
            {
                splitterBorderColor = value;
                this.Invalidate();
            }
        }
        #endregion

        #region SplitterPointCount
        int splitterPointCount = 5;
        /// <summary>
        /// Ayıraç üzerinde gösterilen nokta sayısı
        /// </summary>
        [Browsable(true)]
        [Description("Splitter içerisindeki nokta sayısı")]
        [Category("MT Kontrol")]
        [DefaultValue(5)]
        public int SplitterPointCount
        {
            get { return splitterPointCount; }
            set
            {
                splitterPointCount = value;
                this.Invalidate();
            }
        }
        #endregion

        #region PaintAngle
        private float splitterPaintAngle = 90F;
        /// <summary>
        /// Get/Set, Splitter boyama açısı
        /// </summary>
        [Browsable(true)]
        [Description("Splitter boyama açısı")]
        [Category("MT Kontrol")]
        public float SplitterPaintAngle
        {
            get { return splitterPaintAngle; }
            set
            {
                splitterPaintAngle = value;
                this.Invalidate();
            }
        }
        #endregion

        #region Yapıcı Metodlar

        public MTSplitter()
        {
            this.Size = new Size(4, 100);

            this.SetCursorStyle();

            // DockStyle değiştirildiğinde
            this.DockChanged += delegate
            {
                this.SetCursorStyle();
            };

            // Sistem renkleri değiştirildiğinde
            this.SystemColorsChanged += delegate
            {
                this.SplitterDarkColor = ProfessionalColors.SeparatorDark;
                this.SplitterLightColor = ProfessionalColors.SeparatorLight;
            };

            // Tekrar boyutlandırıldığında
            this.Resize += delegate(object sender, EventArgs e)
            {
                this.Invalidate();
            };

        }


        void SetCursorStyle()
        {
            // Cursor Dock duruma göre değiştir

            if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right) // Dik durumda ise
                Cursor = Cursors.SizeWE;
            else if (this.Dock == DockStyle.Bottom || this.Dock == DockStyle.Top) // Yatay durumda ise
                Cursor = Cursors.SizeNS;
            else
                Cursor = Cursors.Default; // Kaplamış yada hiçbiri ise
        }

        #endregion

        #region OnPaintBackground : Splitter için ayıraç çizgisi oluşturma
        /// <summary>
        /// Splitter için nokta işaretlerini oluştur
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {

            base.OnPaintBackground(pevent);

            Rectangle splitRectangle = this.ClientRectangle;

            if (!(splitRectangle.Width > 0 && splitRectangle.Height > 0))
                return;

            // Ters renk splitter içerisinde gözükmesi için

            Brush koyuRenk = new SolidBrush(this.SplitterDarkColor);
            Brush acikRenk = new SolidBrush(this.SplitterLightColor);

            // Eğer splitter dikey konumda ise 
            // Not : Splitter nesnesi DockStyle.Fill yada DockStyle.None değerlerini almıyor

            if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right) // Dikey konumda ise
            {
                // Splitter 3D görünümü ver
                using (Brush b = new LinearGradientBrush(splitRectangle, this.SplitterLightColor, this.SplitterDarkColor, this.SplitterPaintAngle))
                {
                    pevent.Graphics.FillRectangle(b, splitRectangle);
                }

                int noktaBoyut = 5, noktaYukseklik = 2;
                int ilkNoktaKoor = (splitRectangle.Height - (this.SplitterPointCount * noktaBoyut)) / 2;
                int noktaLeft = (int)(this.Width / 2);

                // Kareleri oluştur
                for (int i = 0; i < this.SplitterPointCount; i++)
                {
                    // Noktanın koyu rengi
                    pevent.Graphics.FillRectangle(koyuRenk, noktaLeft, ilkNoktaKoor, noktaYukseklik, noktaYukseklik);
                    // Noktanın açık rengi
                    pevent.Graphics.FillRectangle(acikRenk, noktaLeft, ilkNoktaKoor + 1, noktaYukseklik, noktaYukseklik);
                    // Siyah Nokta
                    pevent.Graphics.FillRectangle(SystemBrushes.GrayText, noktaLeft, ilkNoktaKoor - 1, noktaYukseklik, noktaYukseklik);
                    ilkNoktaKoor += noktaBoyut;
                }


            }
            else if (this.Dock == DockStyle.Bottom || this.Dock == DockStyle.Top) // Eğer splitter yatay durumda ise
            {

                using (Brush b = new LinearGradientBrush(splitRectangle, this.SplitterLightColor, this.SplitterDarkColor, this.SplitterPaintAngle))
                {
                    pevent.Graphics.FillRectangle(b, splitRectangle);
                }

                int noktaBoyut = 4, noktaYukseklik = 2;
                int ilkNoktaKoor = (splitRectangle.Width - (this.SplitterPointCount * noktaBoyut)) / 2;
                int Y = (int)((splitRectangle.Height - 1) / 2);

                // Kareleri oluştur
                for (int i = 0; i < this.SplitterPointCount; i++)
                {
                    pevent.Graphics.FillRectangle(koyuRenk, ilkNoktaKoor, Y, noktaYukseklik, noktaYukseklik);
                    pevent.Graphics.FillRectangle(acikRenk, ilkNoktaKoor + 1, Y + 1, noktaYukseklik, noktaYukseklik);
                    pevent.Graphics.FillRectangle(SystemBrushes.GrayText, ilkNoktaKoor + 1, Y, noktaYukseklik, noktaYukseklik);

                    ilkNoktaKoor += noktaBoyut;
                }

            }

            pevent.Graphics.DrawRectangle(new Pen(this.SplitterBorderColor),
                new Rectangle(0, 0, Width - 1, Height));


            if (koyuRenk is IDisposable)
                koyuRenk.Dispose();

            if (acikRenk is IDisposable)
                acikRenk.Dispose();

        }

        #endregion

    }
}
