using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace MTM_RE_160.Devices
{

    /// <summary>
    /// КЛАСС загрузчик с МТМ РЭ-160
    /// </summary>
    public class DownLoadsMTMData
    {
        #region ПРИВАТНЫЕ СВОЙСТВА
        /// <summary>
        /// Задержка между запросами на канал
        /// </summary>
        int DelayBetweenRequests { get; set; }

        /// <summary>
        /// Время ожидания ответа
        /// </summary>
        int ResponseTime { get; set; }

        /// <summary>
        /// Количество запросов при ожидании
        /// </summary>
        int ResponseCount { get; set; } 
        #endregion

        #region ПОЛЯ
        /// <summary>
        /// КОД ЗАВЕРШЕНИЯ СНЯТИЯ отправляется в конце передачи 
        /// </summary>
        const byte ENDGETCODE = 0x04;           //Завершение снятия
        /// <summary>
        /// КОД Получения первого блока
        /// </summary>
        const byte GETFIRSTBLOCK = 0x02;        //Получение первого блока
        /// <summary>
        /// КОД получения следующего блока
        /// </summary>
        const byte GETNEXTBLOCK = 0x17;         //Получение следующего блока
        /// <summary>
        /// КОД получения текущего блока
        /// </summary>
        const byte GETCURRENTBLOCK = 0x18;      //Получение текущего блока
        /// <summary>
        /// Длина принимаемых данных
        /// </summary>
        const int BLOCKLENGTH = 512;            //Размер блока
        /// <summary>
        /// Экземпляр МТМ РЭ-160
        /// </summary>
        MTMDevice mtm;
        #endregion
        Thread thread;
        SerialPort port;
        #region КОНСТРУКТОР
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="mtm">МТМ РЭ-160</param>
        /// <param name="DelayBetweenRequests">Задержка между запросами на канал</param>
        /// <param name="ResponseTime">Время ожидания ответа</param>
        /// <param name="ResponseCount">Количество запросов при ожидании</param>
        public DownLoadsMTMData(MTMDevice mtm)
        {
            int DelayBetweenRequests = Settings.DelayBetweenRequests;
            int ResponseTime = Settings.ResponseTime;
            int ResponseCount = Settings.ResponseCount;
            this.mtm = mtm;
            this.DelayBetweenRequests = DelayBetweenRequests;
            this.ResponseTime = ResponseTime;
            this.ResponseCount = ResponseCount;
        }
        #endregion

        #region ПОЛУЧЕНИЕ ДАННЫХ
        /// <summary>
        /// Получить данные
        /// </summary>
        public void Start()
        {
            thread = new Thread(Download);
            thread.Start();
        }
        public void Canseled()
        {
            if (thread != null && thread.IsAlive)
            {
               
                if (port!=null&&port.IsOpen) port.Close();
                Ended?.Invoke("Остановлен", bytesreaded);
                thread.Abort();
            }
        }
        /// <summary>
        /// Снять данные
        /// </summary>
        private void Download()
        {
            port = new SerialPort();
            port.BaudRate = mtm.SpeedPort;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            bytesreaded = new List<byte[]>();
            port.PortName = mtm.PortName;
            try
            {
                if (port.IsOpen) port.Close();
                port.Open();
            }
            catch {
                Ended?.Invoke("COM-порт занят", bytesreaded);
                thread.Abort();
                if (port.IsOpen) port.Close();
                return;
            }
            Started?.Invoke("Начали");
            for (byte channel = 0; channel < 6; channel++)
                GETDATA(port, mtm.Address, channel, mtm.Blocks);

            port.Close();
            Ended?.Invoke("Закончили", bytesreaded);
            thread.Abort();

        }

        /// <summary>
        /// Получить данные с канала
        /// </summary>
        /// <param name="port">Порт</param>
        /// <param name="address">Адрес</param>
        /// <param name="channel">Канал</param>
        /// <param name="blocks">Количество блоков</param>
        private void GETDATA(SerialPort port, byte address, byte channel, byte blocks)
        {
            StatusAllCount?.Invoke((blocks) * 6);

            #region ЗАПРОС доступен ли прибор
            port.Parity = Parity.Space;
            port.Write(new byte[1] { address }, 0, 1);
            Message?.Invoke(mtm, " Запрос");
            int count = 0;
            for (int sendCount = 0; sendCount < ResponseCount; sendCount++)
            {
                Thread.Sleep(ResponseTime);
                count = port.BytesToRead;
                if (port.BytesToRead != 0) break;
            }
            byte[] bytesForRead = new byte[count];
            port.Read(bytesForRead, 0, count);
            //Thread.Sleep(DelayBetweenRequests * 10);
            #endregion

            #region ЗАПРОС на канал
            port.Parity = Parity.Mark;
            port.Write(new byte[1] { channel }, 0, 1);
            count = 0;
            for (int sendCount = 0; sendCount < ResponseCount; sendCount++)
            {
                Thread.Sleep(ResponseTime);
                count = port.BytesToRead;
                if (port.BytesToRead != 0) break;
            }
            if (count == 0)            
            {
                Message?.Invoke(mtm, "МТМ не отвечает");
                port.Parity = Parity.Mark;
                port.Write(new byte[1] { ENDGETCODE }, 0, 1);
                Thread.Sleep(DelayBetweenRequests * 100);
                return;
            }
            bytesForRead = new byte[count];
            port.Read(bytesForRead, 0, count);
            Message?.Invoke(mtm, "МТМ найден");
            Thread.Sleep(DelayBetweenRequests);
            #endregion

            #region ЗАПРОС первый блок
            port.Parity = Parity.Mark;
            port.Write(new byte[1] { GETFIRSTBLOCK }, 0, 1);
            count = 0;
            BlockLoad?.Invoke(mtm, port, address, (byte)(channel + 1), 1, "Снимаем", false);
            for (int sendCount = 0; sendCount < ResponseCount; sendCount++)
            {
                Thread.Sleep(ResponseTime);
                count = port.BytesToRead;
                if (port.BytesToRead == BLOCKLENGTH) break;
            }
            bytesForRead = new byte[count];
            port.Read(bytesForRead, 0, count);

            if (count != BLOCKLENGTH)
            {
                for (int repeat = 1; repeat < 4; repeat++)
                {
                    port.Parity = Parity.Mark;
                    port.Write(new byte[1] { GETCURRENTBLOCK }, 0, 1);
                    count = 0;
                     for (int sendCount = 0; sendCount < ResponseCount; sendCount++)
                    {
                        Thread.Sleep(ResponseTime);
                        count = port.BytesToRead;
                        if (port.BytesToRead == BLOCKLENGTH) goto loop1;
                    }
                }               
            }
        loop1:
            if (bytesForRead != null&& bytesForRead.Length==BLOCKLENGTH) bytesreaded.Add(bytesForRead);
            Thread.Sleep(DelayBetweenRequests);
            if (count != 0) {
                BlockLoad?.Invoke(mtm, port, address, (byte)(channel + 1), 1, "Получен", true);
            }
            else BlockLoad?.Invoke(mtm, port, address, (byte)(channel + 1), 1, "Пропущен", false);
            #endregion

            #region ЗАПРОС следующий блок
            for (byte block = 1; block < blocks; block++)
            {
                BlockLoad?.Invoke(mtm, port, address, (byte)(channel + 1), (byte)(block + 1), "Снимаем", false);
                port.Parity = Parity.Mark;
                port.Write(new byte[1] { GETNEXTBLOCK }, 0, 1);
                count = 0;
                for (int sendCount = 0; sendCount < ResponseCount; sendCount++)
                {
                    Thread.Sleep(ResponseTime);
                    count = port.BytesToRead;
                    if (port.BytesToRead == BLOCKLENGTH) break;
                }
                if (count != BLOCKLENGTH)
                {
                    for (int repeat = 1; repeat < 4; repeat++)
                    {
                        port.Parity = Parity.Mark;
                        port.Write(new byte[1] { GETCURRENTBLOCK }, 0, 1);
                        count = 0;
                        BlockLoad?.Invoke(mtm, port, address, (byte)(channel + 1), (byte)(block + 1), "повтор " + repeat, false);
                        for (int sendCount = 0; sendCount < ResponseCount; sendCount++)
                        {
                            Thread.Sleep(ResponseTime);
                            count = port.BytesToRead;
                            if (port.BytesToRead == BLOCKLENGTH) goto loop;
                        }
                    }
                    continue;
                }
            loop:
                bytesForRead = new byte[count];
                port.Read(bytesForRead, 0, count);
                if (bytesForRead != null && bytesForRead.Length == BLOCKLENGTH) bytesreaded.Add(bytesForRead);
                if (count != 0)
                {
                    BlockLoad?.Invoke(mtm, port, address, (byte)(channel + 1), (byte)(block + 1), "Получен", true);
                }
                else BlockLoad?.Invoke(mtm, port, address, (byte)(channel + 1), (byte)(block + 1), "Пропущен", false);

            }
            Thread.Sleep(500);
            #endregion

            #region ЗАПРОС окончание
            port.Parity = Parity.Mark;
            port.Write(new byte[1] { ENDGETCODE }, 0, 1);
            port.Write(new byte[1] { ENDGETCODE }, 0, 1);
            port.Write(new byte[1] { ENDGETCODE }, 0, 1);
            Thread.Sleep(1000);

            #endregion
        }
        #endregion

        public  Action<MTMDevice, string> Message;
        public  Action<MTMDevice, SerialPort, byte, byte, byte, string, bool> BlockLoad;
        public Action<int> StatusAllCount;
        public Action<string> Started;
        public Action<string, List<byte[]>> Ended;
        List<byte[]> bytesreaded;
    }





}
