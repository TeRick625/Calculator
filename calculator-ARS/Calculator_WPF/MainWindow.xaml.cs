using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator_WPF
{
    public partial class MainWindow : Window
    {

        private List<double?> _tbNumbers = new List<double?>(2);
        private double? _tbMemory;
        private bool IsMemoryFull = false;
        private string _operation { get; set; }
        private string _oldOperation { get; set; }

        private bool isBlocked = false;

        public MainWindow()
        {
            InitializeComponent();
            _tbNumbers.Add(null);
            _tbNumbers.Add(null);
        }


        private async void Error()
        {
            tbResult.IsEnabled = false;
            tbResult.Text = "Error!";
            await Task.Delay(300);

            tbResult.Text = String.Empty;
            tbResult.IsEnabled = true;
        }
        private void DoOperation(string operation)
        {
            try
            {

                //Проверяем операцию
                if (operation != "=" && operation != "%")
                    _oldOperation = operation;
                _operation = operation;

                if (_operation.Equals("="))
                {
                    //Повторяем преведущую операцию 
                    DoMath(_oldOperation);
                    isBlocked = true;
                }
                else
                {
                    //Делаем текущую операцию
                    DoMath(_operation);
                }
            }
            catch (Exception)
            {
                Error();
            }
        }

        private void DoMath(string operation)
        {
            if (string.IsNullOrEmpty(operation))
            {
                WriteToLog("Choose operation!");
                return;
            }
            if (String.IsNullOrEmpty(tbResult.Text))
                tbResult.Text = "0";

            if (_tbNumbers[0] == null)
            {
                _tbNumbers[0] = Convert.ToDouble(tbResult.Text);
                //Решаем
                double? result = null;
                switch (operation)
                {
                    case "|x|":
                        result = Math.Abs((double)_tbNumbers[0]);
                        break;

                    case "1/x":
                        result = 1 / _tbNumbers[0];
                        break;
                    case "x^2":
                        result = Math.Pow((double)_tbNumbers[0], 2);
                        break;
                    case "√x":
                        result = Math.Sqrt((double)_tbNumbers[0]);
                        break;
                    default:
                        WriteToLog("Choose operation!");
                        break;
                }

                //Выводим в Log
                WriteToLog(_tbNumbers[0], operation, _tbNumbers[1], result);
                //Присваеваем число
                _tbNumbers[0] = result ?? _tbNumbers[0];
                //Чистим tbResult
                tbResult.Text = result.ToString();

                var operations = new string[] { "√x", "x^2", "1/x", "|x|"};
                if (operations.Contains(operation))
                {
                    _tbNumbers[0] = null;
                    _tbNumbers[1] = null;
                }

            }
            else if (_tbNumbers[1] == null)
            {
                _tbNumbers[1] = Convert.ToDouble(tbResult.Text);

                double? result = null;

                switch (operation)
                {
                    //Присудствует другая кодировка символов
                    case "+":
                        result = _tbNumbers[0] + _tbNumbers[1];
                        break;

                    case "−":
                        result = _tbNumbers[0] - _tbNumbers[1];
                        break;

                    case "÷":
                        result = _tbNumbers[0] / _tbNumbers[1];
                        break;

                    case "×":
                        result = _tbNumbers[0] * _tbNumbers[1];
                        break;

                    case "=":
                        result = _tbNumbers[1];
                        break;

                    case "|x|":
                        result = Math.Abs((double)_tbNumbers[1]);
                        break;
                    case "1/x":
                        result = 1 / _tbNumbers[1];
                        break;
                    case "x^2":
                        result = Math.Pow((double)_tbNumbers[1], 2);
                        break;
                    case "√x":
                        result = Math.Sqrt((double)_tbNumbers[1]);
                        break;
                    case "%":
                        tbResult.Text = $"{_tbNumbers[0] / _tbNumbers[1]}";
                        _tbNumbers[1] = null;
                        DoOperation(_oldOperation);
                        return;

                    default:
                        Error();
                        break;
                }

                // Для того чтобы коректно выводилсь информация
                var operations = new string[] { "√x", "x^2", "1/x", "|x|"};
                if (operations.Contains(operation))
                {
                    _tbNumbers[0] = null;
                    _tbNumbers[1] = null;
                    DoMath(operation);
                    return;
                }

                WriteToLog(_tbNumbers[0], operation, _tbNumbers[1], result);

                _tbNumbers[0] = null;
                _tbNumbers[1] = null;

                tbResult.Text = result.ToString();
            }
        }

        void WriteToLog(string text)
        {
            lLog.Content = $"{text}";
        }

        private void WriteToLog(double? n1, string operation, double? n2 = null, double? result = null)
        {
            string n1Str = n1.ToString() ?? "";
            string n2Str = n2.ToString() ?? "";
            string resultStr = result.ToString() ?? "";

            lLog.Content = $"{n1Str}({operation}){n2Str}" + (string.IsNullOrEmpty(resultStr) ? "" : $"={resultStr}");
        }
        private void FirstrNumValueSet()
        {
            if (tbResult.Text == "0")
            {
                tbResult.Text = "";
            }
        }

        private void fAddToTb(Button bNumber)
        {
            if (!isBlocked)
            {
                FirstrNumValueSet();
                tbResult.Text = tbResult.Text + bNumber.Content;
            }
        }


        private void Mi_Program_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //Calc
        private void Btn_FuncPrecent_Click(object sender, RoutedEventArgs e)
        {
            string operation = ((Button)sender).Content.ToString();
            DoOperation(operation);
        }

        private void Btn_FuncClear_Click(object sender, RoutedEventArgs e)
        {
            tbResult.Text = $"{0}";
            isBlocked = false;
        }

        private void Btn_FuncClearAll_Click(object sender, RoutedEventArgs e)
        {
            WriteToLog("");
            _tbNumbers[0] = null;
            _tbNumbers[1] = null;
            tbResult.Text = $"{0}";
            isBlocked = false;
        }

        private void Btn_FuncDel_Click(object sender, RoutedEventArgs e)
        {
            string text = tbResult.Text;
            if (String.IsNullOrEmpty(text) || text.Length == 1 || (text[0].Equals('-') && text.Length == 2))
            {
                tbResult.Text = $"{0}";
                return;
            }
            else
            {
                tbResult.Text = text.Remove(text.Length - 1);
                return;
            }
        }


        private void Btn_FuncLog_Click(object sender, RoutedEventArgs e)
        {
            string operation = ((Button)sender).Content.ToString();
            DoOperation(operation);
        }

        private void Btn_FuncModul_Click(object sender, RoutedEventArgs e)
        {
            string operation = ((Button)sender).Content.ToString();
            DoOperation(operation);
        }

        private void Btn_Func1SplitX_Click(object sender, RoutedEventArgs e)
        {
            string operation = ((Button)sender).Content.ToString();
            DoOperation(operation);
        }

        private void Btn_FuncPowX_Click(object sender, RoutedEventArgs e)
        {
            string operation = ((Button)sender).Content.ToString();
            DoOperation(operation);
        }

        private void Btn_FuncSqrtX_Click(object sender, RoutedEventArgs e)
        {
            string operation = ((Button)sender).Content.ToString();
            DoOperation(operation);
        }

        private void Btn_FuncSplit_Click(object sender, RoutedEventArgs e)
        {
            string operation = ((Button)sender).Content.ToString();
            DoOperation(operation);
            isBlocked = false;
        }

        private void Btn_Number0_Click(object sender, RoutedEventArgs e)
        {
            fAddToTb((Button)sender);
        }
        private void Btn_Number1_Click(object sender, RoutedEventArgs e)
        {
            fAddToTb((Button)sender);
        }
        private void Btn_Number2_Click(object sender, RoutedEventArgs e)
        {
            fAddToTb((Button)sender);
        }
        private void Btn_Number3_Click(object sender, RoutedEventArgs e)
        {
            fAddToTb((Button)sender);
        }
        private void Btn_Number4_Click(object sender, RoutedEventArgs e)
        {
            fAddToTb((Button)sender);
        }
        private void Btn_Number5_Click(object sender, RoutedEventArgs e)
        {
            fAddToTb((Button)sender);
        }
        private void Btn_Number6_Click(object sender, RoutedEventArgs e)
        {
            fAddToTb((Button)sender);
        }
        private void Btn_Number7_Click(object sender, RoutedEventArgs e)
        {
            fAddToTb((Button)sender);
        }
        private void Btn_Number8_Click(object sender, RoutedEventArgs e)
        {
            fAddToTb((Button)sender);
        }
        private void Btn_Number9_Click(object sender, RoutedEventArgs e)
        {
            fAddToTb((Button)sender);
        }

        private void Btn_FuncMultiply_Click(object sender, RoutedEventArgs e)
        {
            string operation = ((Button)sender).Content.ToString();
            DoOperation(operation);
            isBlocked = false;
        }

        private void Btn_FuncMinus_Click(object sender, RoutedEventArgs e)
        {
            string operation = ((Button)sender).Content.ToString();
            DoOperation(operation);
            isBlocked = false;
        }

        private void Btn_FuncPlus_Click(object sender, RoutedEventArgs e)
        {
            string operation = ((Button)sender).Content.ToString();
            DoOperation(operation);
            isBlocked = false;
        }

        private void Btn_FuncEquals_Click(object sender, RoutedEventArgs e)
        {
            string operation = ((Button)sender).Content.ToString();
            DoOperation(operation);
        }

        private void Btn_NumberInversion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double number = Convert.ToDouble(tbResult.Text);
                number *= -1;
                tbResult.Text = number.ToString();
            }
            catch (Exception)
            {
                Error();
            }
        }

        private void Btn_NumberPoint_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(tbResult.Text)) tbResult.Text = "0";

            tbResult.Text = tbResult.Text.Replace(",", "");

            fAddToTb(btn_NumberPoint);
        }

        private void btn_FuncMC_Click(object sender, RoutedEventArgs e)
        {
            _tbMemory = null;
            IsMemoryFull = false;
            lMem.Content = "";

        }

        private void btn_FuncMS_Click(object sender, RoutedEventArgs e)
        {
            if (!(String.IsNullOrEmpty(tbResult.Text) || tbResult.Text == "0" || tbResult.Text == "Error!" || tbResult.Text == "∞" || tbResult.Text.Contains("не")))
            {
                _tbMemory = Convert.ToDouble(tbResult.Text);
                IsMemoryFull = true;
                WriteToMEM(_tbMemory.ToString());
            }
        }

        private void btn_FuncMW_Click(object sender, RoutedEventArgs e)
        {
            if (!(String.IsNullOrEmpty(tbResult.Text) || tbResult.Text == "0" || tbResult.Text == "Error!" || tbResult.Text == "∞" || tbResult.Text.Contains("не")) && IsMemoryFull)
            {
                tbResult.Text = _tbMemory.ToString();
            }
        }

        private void btn_FuncMP_Click(object sender, RoutedEventArgs e)
        {
            if (!(String.IsNullOrEmpty(tbResult.Text) || tbResult.Text == "0" || tbResult.Text == "Error!" || tbResult.Text == "∞" || tbResult.Text.Contains("не")) && IsMemoryFull)
            {
                _tbMemory += Convert.ToDouble(tbResult.Text);
                WriteToMEM(_tbMemory.ToString());
            }
        }

        private void btn_FuncMM_Click(object sender, RoutedEventArgs e)
        {
            if (!(String.IsNullOrEmpty(tbResult.Text) || tbResult.Text == "0" || tbResult.Text == "Error!" || tbResult.Text == "∞" || tbResult.Text.Contains("не")) && IsMemoryFull)
            {
                _tbMemory -= Convert.ToDouble(tbResult.Text);
                WriteToMEM(_tbMemory.ToString());
            }
        }

        private void WriteToMEM(string memNumber)
        {
            lMem.Content = memNumber;
        }
    }
}
