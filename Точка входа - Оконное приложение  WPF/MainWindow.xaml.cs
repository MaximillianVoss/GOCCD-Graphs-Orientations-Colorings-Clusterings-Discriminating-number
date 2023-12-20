using GraphBase.Генераторы;
using GraphBase.Графы;
using GraphBase.Параметры;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace Точка_входа___Оконное_приложение__WPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        #region Поля
        // Словарь для хранения числа графов для заданного количества вершин
        Dictionary<int, long> graphCounts = new Dictionary<int, long>
        {
            {0, 1}, // Сдвинуто на 1 назад: было 1, стало 0
            {1, 1}, // Сдвинуто на 1 назад: было 2, стало 1
            {2, 1}, // Сдвинуто на 1 назад: было 3, стало 2
            {3, 2}, // и так далее
            {4, 6},
            {5, 21},
            {6, 112},
            {7, 853},
            {8, 11117},
            {9, 261080},
            {10, 11716571},
            {11, 1006700565},
            {12, 164059830476},
            {13, 50335907869219},
            {14, 29003487462848061},
                    // Продолжайте шаблон для других значений, уменьшая
        };
        int cmbGraphTypeIndex = 0;
        #endregion

        #region Свойства

        #endregion

        #region Методы
        void Init()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.txbVertexCount.ValidationText = "Введенное значение не является числом!";
            this.txbVertexCount.RegEx = "^\\d+$";

            this.txbColorsCount.ValidationText = "Введенное значение не является числом!";
            this.txbColorsCount.RegEx = "^\\d+$";

            this.cmbMethod.Add("Генерация с помощью вектора степеней");
            this.cmbMethod.Add("Генерация с помощью канонического кода");
            this.cmbMethod.SelectedIndex = 0;
        }

        private async void Start()
        {
            // Проверка введённых данных
            if (!int.TryParse(this.txbVertexCount.Text, out int vertexCount) || vertexCount <= 0 || !this.txbVertexCount.IsValid)
            {
                MessageBox.Show("Введенное значение не является положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.cmbMethod.SelectedIndex == -1)
            {
                MessageBox.Show("Не выбран метод генерации!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }



            // Создание генератора и выбор метода генерации
            var generator = new GeneratorNaughty(vertexCount);
            string generationMethod = this.cmbMethod.SelectedIndex == 0 ? "Вектор степеней" : "Канонический код";
            //string graphType = this.cmbGraphType.SelectedIndex == 0 ? "Пользовательский" : "QuickGraph";
            string graphType = cmbGraphTypeIndex == 0 ? "Пользовательский" : "QuickGraph";

            // Подготовка к сохранению файла
            string reportFileName = $"Отчет граф {vertexCount} вершин - {generationMethod} - {graphType}.txt";

            var saveFileDialog = new SaveFileDialog
            {
                FileName = reportFileName, // Начальное имя файла
                DefaultExt = ".txt", // Расширение файла по умолчанию
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*" // Фильтр форматов файлов
            };

            // Показать диалог сохранения файла
            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = saveFileDialog.FileName;

                // Запускаем асинхронную операцию
                await Task.Run(() =>
                {
                    using (var reportFile = new StreamWriter(filename))
                    {
                        var totalStopwatch = Stopwatch.StartNew();
                        int graphNumber = 1;

                        // Проверяем, есть ли в словаре количество графов для данного числа вершин
                        if (!graphCounts.TryGetValue(vertexCount, out long totalGraphs))
                        {
                            throw new ArgumentException("Для данного количества вершин нет информации о числе графов.");
                        }

                        // Инициализация ProgressBar
                        this.Dispatcher.Invoke(() =>
                        {
                            pbProgress.Maximum = totalGraphs;
                            pbProgress.Value = 0;
                        });

                        // Обработка графов
                        foreach (string g6 in generator.GenerateAllGraphsG6(vertexCount, GeneratorType.CONNECTED_GRAPHS))
                        {
                            // Ваши операции с графом...
                            var adjacencyMatrix = new G6String(g6).ToAdjacencyMatrix();
                            Graph graph = null;

                            switch (this.cmbMethod.SelectedIndex)
                            {
                                case 0:
                                    var degreeVector = adjacencyMatrix.ToDegreeVector();
                                    //graph = graphTypeChoice == 1 ? new GraphCustom(degreeVector) : new GraphQuickGraph(adjacencyMatrix.Matrix);
                                    graph = new GraphCustom(degreeVector);
                                    break;
                                case 1:
                                    var canonicalGraphCode = adjacencyMatrix.ToCanonicalGraphCode();
                                    //graph = graphTypeChoice == 1 ? new GraphCustom(canonicalGraphCode) : new GraphQuickGraph(adjacencyMatrix.Matrix);
                                    graph = new GraphCustom(canonicalGraphCode);
                                    break;
                                default:
                                    throw new InvalidOperationException("Неизвестный метод генерации графов.");
                            }

                            // Выводим номер графа перед информацией
                            string graphNumStr = $"Граф #{graphNumber}:";
                            Console.Write(graphNumStr);
                            reportFile.Write(graphNumStr);


                            string graphInfo = graph.ToString();
                            Console.WriteLine(graphInfo);
                            reportFile.WriteLine(graphInfo);
                            // Обновление ProgressBar
                            this.Dispatcher.Invoke(() =>
                            {
                                pbProgress.Value = graphNumber;
                            });

                            graphNumber++;
                        }

                        totalStopwatch.Stop();
                        string totalTimeInfo = $"Общее время выполнения: {totalStopwatch.ElapsedMilliseconds} мс";

                        this.Dispatcher.Invoke(() =>
                        {
                            reportFile.WriteLine(totalTimeInfo);
                            MessageBox.Show(totalTimeInfo, "Завершено", MessageBoxButton.OK, MessageBoxImage.Information);
                        });
                    }
                });
            }
            else
            {
                MessageBox.Show("Сохранение отчета было отменено.", "Отмена", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        #region Конструкторы/Деструкторы
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }
        #endregion

        #region Операторы

        #endregion

        #region Обработчики событий
        private void ButtonPrimary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run(() => this.Start());
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }
        #endregion





    }
}
