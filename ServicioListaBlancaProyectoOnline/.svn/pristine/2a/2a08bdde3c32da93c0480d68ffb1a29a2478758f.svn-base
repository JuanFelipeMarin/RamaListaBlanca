using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Sibo.WhiteList.IngresoTouch.Classes
{
    class clsLectorPalma
    {
        IntPtr m_hDevice = IntPtr.Zero;

        static int MAX_IMAGE_SIZE = 2 * 8 * 1024 * 1024;
        static int RAW_TEMPLATELEN = 99120;

        static int MERGE_TEMPLATELEN = 8844;
        static int MATH_TEMPLATELEN = 27120;
        IntPtr[] m_preRegTemplates = new IntPtr[ENROLL_CNT];
        string errorConexionInt = "";

        static int ZKPALM_ERR_OK = 0;
        public bool m_bStop = false;
        public bool m_bRegister = false;

        static int ENROLL_CNT = 5;
        static int DEF_WIDTH = 480;
        static int DEF_HEIGHT = 640;

        int m_nEnrollIdx = 0;
        int m_nWidth = DEF_WIDTH;
        int m_nHeight = DEF_HEIGHT;

        static int PARAM_CODE_WIDTH = 1;
        static int PARAM_CODE_HEIGHT = 2;

        static int TurnOffLED = 0x00;

        int[] m_nPreRegTemplateSize = new int[ENROLL_CNT];

        byte[] m_verTemplate = new byte[MATH_TEMPLATELEN];
        byte[] m_rawTemplate = new byte[RAW_TEMPLATELEN];

        Thread m_hThreadCapture = null;
        byte[] m_pImgBuffer = new byte[MAX_IMAGE_SIZE];


        public byte[] regExitoso;
        public bool registroConErrores = false;

        MemoryStream ms = new MemoryStream();
        const int MESSAGE_CAPTURE_OK = 0x0400 + 6;
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_GetVersion(byte[] version, int size);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_Init();

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_Terminate();

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_OpenDevice(int index, ref IntPtr handle);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_GetDeviceCount(ref int devcnt);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_CloseDevice(IntPtr handle);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_SetParameter(IntPtr handle, int paramCode, byte[] paramValue, int size);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_GetParameter(IntPtr handle, int paramCode, byte[] paramValue, ref int size);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_CapturePalmImageAndTemplate(IntPtr handle, byte[] imgBuffer, int cbImgBuffer, int extractType, byte[] rawTemplate, ref int cbRawTemplate, byte[] verTemplate, ref int cbVerTemplate, ref int quality, int[] pZKPalmRect, IntPtr inreserved);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_Verify(IntPtr handle, byte[] regTemplage, int cbRegTemplate, byte[] verTemplate, int cbVerTemplate, ref int score);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_VerifyByID(IntPtr handle, byte[] verTemplate, int cbVerTemplate, byte[] id, ref int score);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_MergeTemplates(IntPtr handle, IntPtr[] rawTemplates, int mergedCount, byte[] mergeTemplate, ref int cbMergeTemplate);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_DBAdd(IntPtr handle, byte[] id, byte[] pRegTemplate, int cbRegTemplate);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_DBDel(IntPtr handle, byte[] id);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_DBCount(IntPtr handle, ref int count);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_DBClear(IntPtr handle);

        [DllImport("ZKPalmAPI.dll")]
        extern static int ZKPalm_DBIdentify(IntPtr handle, byte[] verTemplate, int cbVerTemplate, byte[] id, ref int score, int minScore, int maxScore);

        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        public static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        public string errorConexion
        {
            get { return errorConexionInt; }
            set { errorConexionInt = value; }
        }

        public bool conectarDispositivo()
        {
            if (IntPtr.Zero != m_hDevice)
            {
                errorConexion = "El dispositivo ya esta conectado!";
                return false;
            }
            int ret = ZKPalm_Init();
            if (ZKPALM_ERR_OK != ret)
            {
                errorConexion = "No se pudo inicializar, ret= " + ret;
                return false;
            }
            ret = ZKPalm_OpenDevice(0, ref m_hDevice);
            if (ZKPALM_ERR_OK != ret)
            {
                errorConexion = "No se pudo abrir el dispositivo, ret= " + ret;
                return false;
            }

            m_bStop = false;

            byte[] width = new byte[4];
            byte[] height = new byte[4];
            int paramSize = width.Length;

            ZKPalm_GetParameter(m_hDevice, PARAM_CODE_WIDTH, width, ref paramSize);
            ZKPalm_GetParameter(m_hDevice, PARAM_CODE_HEIGHT, height, ref paramSize);

            m_nWidth = BitConverter.ToInt32(width, 0);
            m_nHeight = BitConverter.ToInt32(height, 0);

            for (int i = 0; i < m_preRegTemplates.Length; i++)
            {
                m_preRegTemplates[i] = Marshal.AllocHGlobal(RAW_TEMPLATELEN);
            }
            return true;
        }

        public void actualizarBaseDatos(DataTable dtHuella)
        {
            if (IntPtr.Zero != m_hDevice)
            {
                int ret = ZKPalm_DBClear(m_hDevice);
                if (0 == ret)
                {
                    foreach (DataRow fila in dtHuella.Rows)
                    {
                        string strPalmId = fila["personId"].ToString();
                        byte[] bytePalmId = System.Text.Encoding.Default.GetBytes(strPalmId);
                        byte[] regTmp = (byte[])fila["fingerprint"];
                        int nret = ZKPalm_DBAdd(m_hDevice, bytePalmId, regTmp, MERGE_TEMPLATELEN);
                    }
                }

            }
        }

        public string DoVerify(ref System.Windows.Forms.PictureBox pictureBoxPalm)
        {
            Array.Clear(m_verTemplate, 0, MATH_TEMPLATELEN);
            int cbVerTemplate = MATH_TEMPLATELEN;

            Array.Clear(m_rawTemplate, 0, RAW_TEMPLATELEN);
            int cbRawTemplate = RAW_TEMPLATELEN;

            int nQuality = 0;
            int[] rect = new int[8];
            Array.Clear(rect, 0, 8);
            int ret = ZKPalm_CapturePalmImageAndTemplate(m_hDevice, m_pImgBuffer, m_nWidth * m_nHeight, 2, m_rawTemplate, ref cbRawTemplate, m_verTemplate, ref cbVerTemplate, ref nQuality, rect, IntPtr.Zero);
            //ShowNIRImage(ref pictureBoxPalm);
            if (ZKPALM_ERR_OK == ret)
            {
                //DrawPalmLine(rect, ref pictureBoxPalm);
                byte[] szID = new byte[64];
                int nScore = 0;

                ret = ZKPalm_DBIdentify(m_hDevice, m_verTemplate, cbVerTemplate, szID, ref nScore, 576, 1000);
                if (ZKPALM_ERR_OK == ret)
                {
                    string strfaceID = System.Text.Encoding.Default.GetString(szID);
                    return strfaceID;
                }
                else
                {
                    return "Fallo la comparación";
                }
            }
            else
            {
                return "Fallo la captura";
            }
        }
        private void ShowNIRImage(ref System.Windows.Forms.PictureBox pictureBoxPalm)
        {
            pictureBoxPalm.Image = ToGrayBitmap(m_pImgBuffer, m_nWidth, m_nHeight);
            ClearMemory();
        }

        public static Bitmap ToGrayBitmap(byte[] rawValues, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            int stride = bmpData.Stride;
            int offset = stride - width;
            IntPtr iptr = bmpData.Scan0;
            int scanBytes = stride * height;

            int posScan = 0, posReal = 0;
            byte[] pixelValues = new byte[scanBytes];
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    pixelValues[posScan++] = rawValues[posReal++];
                }
                posScan += offset;
            }

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, iptr, scanBytes);
            bmp.UnlockBits(bmpData);

            ColorPalette tempPalette;
            using (Bitmap tempBmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
            {
                tempPalette = tempBmp.Palette;
            }
            for (int i = 0; i < 256; i++)
            {
                tempPalette.Entries[i] = Color.FromArgb(i, i, i);
            }
            bmp.Palette = tempPalette;

            return bmp;
        }
        private void DrawPalmLine(int[] rect, ref System.Windows.Forms.PictureBox pictureBoxPalm)
        {
            using (Graphics g = Graphics.FromHwnd(pictureBoxPalm.Handle))
            {
                Point[] PointArray = new Point[]{ new Point(rect[0], rect[1]),
                    new Point(rect[2], rect[3]),
                    new Point(rect[4], rect[5]),
                    new Point(rect[6], rect[7]),
                    new Point(rect[0], rect[1])};

                g.DrawLines(new Pen(Color.GreenYellow, 1), PointArray);
            }
            pictureBoxPalm.Update();
        }
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }

        int ultimoMensaje = 0;
        public string DoRegister(ref System.Windows.Forms.PictureBox pictureBoxPalm)
        {
            Array.Clear(m_verTemplate, 0, MATH_TEMPLATELEN);
            Array.Clear(m_rawTemplate, 0, RAW_TEMPLATELEN);

            int cbRawTemplate = RAW_TEMPLATELEN;
            int cbVerTemplate = MATH_TEMPLATELEN;

            int nQuality = 0;
            int[] rect = new int[8];
            Array.Clear(rect, 0, 8);

            int ret = ZKPalm_CapturePalmImageAndTemplate(m_hDevice, m_pImgBuffer, m_nWidth * m_nHeight, 1, m_rawTemplate, ref cbRawTemplate, m_verTemplate, ref cbVerTemplate, ref nQuality, rect, IntPtr.Zero);
            ShowNIRImage(ref pictureBoxPalm);
            if (ZKPALM_ERR_OK == ret)
            {
                DrawPalmLine(rect, ref pictureBoxPalm);
                Marshal.Copy(m_rawTemplate, 0, m_preRegTemplates[m_nEnrollIdx], cbRawTemplate);
                m_nPreRegTemplateSize[m_nEnrollIdx] = cbRawTemplate;
                m_nEnrollIdx++;
                if (m_nEnrollIdx >= ENROLL_CNT)
                {
                    byte[] regTmp = new byte[MERGE_TEMPLATELEN];
                    Array.Clear(regTmp, 0, MERGE_TEMPLATELEN);
                    int regTmpLen = MERGE_TEMPLATELEN;
                    int nret = ZKPalm_MergeTemplates(m_hDevice, m_preRegTemplates, ENROLL_CNT, regTmp, ref regTmpLen);
                    if (ZKPALM_ERR_OK == nret)
                    {
                        regExitoso = regTmp;
                        m_bStop = true;
                        return "Palma registrada satisfactoriamente";
                    }
                    else
                    {
                        registroConErrores = true;
                        return "Fución de palmas erronea, código de error = " + nret;
                    }

                    m_nEnrollIdx = 0;
                    m_bRegister = false;
                    return "";
                }
                return "Por favor coloque su palma " + (ENROLL_CNT - m_nEnrollIdx) + " veces!";
            }
            else
            {
                return "Por favor coloque su palma sobre el dispositivo!";
            }
        }

        public bool desconectarDispositivo()
        {
            Terminate();
            return true;
        }

        private bool Terminate()
        {
            if (IntPtr.Zero == m_hDevice)
            {
                return false;
            }
            m_bStop = true;
            if (null != m_hThreadCapture)
            {
                m_hThreadCapture.Join();
            }

            if (IntPtr.Zero != m_hDevice)
            {
                int nParamValue = TurnOffLED;
                byte[] byteParamValue = BitConverter.GetBytes(nParamValue);
                int ret = ZKPalm_SetParameter(m_hDevice, 2004, byteParamValue, byteParamValue.Length);

                ZKPalm_CloseDevice(m_hDevice);
                m_hDevice = IntPtr.Zero;
            }

            for (int i = 0; i < m_preRegTemplates.Length; i++)
            {
                if (IntPtr.Zero != m_preRegTemplates[i])
                {
                    Marshal.FreeHGlobal(m_preRegTemplates[i]);
                    m_preRegTemplates[i] = IntPtr.Zero;
                }
            }
            return true;
        }

    }
}
