using FestpunktDB.Business;
using FestpunktDB.Business.Entities;
using FestpunktDB.Business.EntitiesDeleted;
using FestpunktDB.Business.EntitiesImport;
using Microsoft.EntityFrameworkCore;
using FestpunktDB.Business.DataServices;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FestpunktDB.Business.ImportService;
using Application = Microsoft.Office.Interop.PowerPoint.Application;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
//using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using FestpunktDB.Business.ExportService;


namespace FestpunktDB.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
            LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            InitializeComponent();
            try
            {
                GetPpAsync(PpTable);
                LoadExport();
            }
            catch (OleDbException e)
            {
                var errorMessage = "";
                for (var i = 0; i < e.Errors.Count; i++)
                {
                    errorMessage += "Index # " + i + "\n" + "Message: " + e.Errors[i].Message + "\n" + "NativeError: " +
                                    e.Errors[i].NativeError + "\n" + "Source: " + e.Errors[i].Source + "\n" +
                                    "SQLState: " + e.Errors[i].SQLState + "\n";
                }
            }

        }

        #region Declarations

        public static EntityFrameworkContext DbGlobal = new EntityFrameworkContext();
        public static ExportFilterContext DbFilter = new ExportFilterContext();
        Pp curSelectedPpInBearbeitungFormular;
        Ph curSelectedPhInBearbeitungFormular;
        Pk curSelectedPkInBearbeitungFormular;
        Pl curSelectedPlInBearbeitungFormular;
        Ps curSelectedPsInBearbeitungFormular;
        Pp ppTemp;
        Pp curSelectedPp = new Pp();
        DataGridRow selectedRow;
        List<Pp> selectedPpsInHauptAnsicht = new List<Pp>();
        List<Pk> selectedPksInHauptAnsicht = new List<Pk>();
        List<Pl> selectedPlsInHauptAnsicht = new List<Pl>();
        List<Ps> selectedPssInHauptAnsicht = new List<Ps>();
        List<Ph> selectedPhsInHauptAnsicht = new List<Ph>();
        List<Pp> selectedPpsInBearbeitungsAnsicht = new List<Pp>();
        List<Pk> selectedPksInBearbeitungsAnsicht = new List<Pk>();
        List<Pl> selectedPlsInBearbeitungsAnsicht = new List<Pl>();
        List<Ps> selectedPssInBearbeitungsAnsicht = new List<Ps>();
        List<Ph> selectedPhsInBearbeitungsAnsicht = new List<Ph>();
        List<Pp> selectedPpsInExportAnsicht = new List<Pp>();
        List<Pk> selectedPksInExportAnsicht = new List<Pk>();
        List<Pl> selectedPlsInExportAnsicht = new List<Pl>();
        List<Ps> selectedPssInExportAnsicht = new List<Ps>();
        List<Ph> selectedPhsInExportAnsicht = new List<Ph>();
        List<Pp> curPpCollection = new List<Pp>();
        List<Ph> curPhCollection = new List<Ph>();
        List<Pk> curPkCollection = new List<Pk>();
        List<Pl> curPlCollection = new List<Pl>();
        List<Ps> curPsCollection = new List<Ps>();
        List<Pp> detachedPpList = new List<Pp>();
        List<Ph> detachedPhList = new List<Ph>();
        List<Pk> detachedPkList = new List<Pk>();
        List<Pl> detachedPlList = new List<Pl>();
        List<Ps> detachedPsList = new List<Ps>();
        List<Pp> modifiedPpList = new List<Pp>();
        List<Ph> modifiedPhList = new List<Ph>();
        List<Pk> modifiedPkList = new List<Pk>();
        List<Pl> modifiedPlList = new List<Pl>();
        List<Ps> modifiedPsList = new List<Ps>();
        List<Pp> ppTempList;
        List<Ph> phTempList;
        List<Pk> pkTempList;
        List<Pl> plTempList;
        List<Ps> psTempList;
        DataGrid TableForEditingTemp;
        DataGrid ProtoPpTableTemp;
        public static System.Data.DataTable dataTableforTemp = new System.Data.DataTable();
        public static System.Data.DataTable dataTableForSkizzen = new System.Data.DataTable();
        public static System.Data.DataTable dataTableForMainDatabaseSketches = new System.Data.DataTable();
        public static System.Collections.IEnumerable itemsSource;
        public static string text;
        OpenFileDialog openFileDialogForSketches = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "All Files |*.JPG;*.pdf*;*.ppt;*.pptx"
        };
        List<string> Filenames = new List<string>();
        List<string> Skizzennames = new List<string>();
        string PAD;
        List<IEnumerable<Pp>> ppPagedList;
        List<IEnumerable<Pp>> ppExportPagedList;
        bool _skipEvent;
        string jpgFile;
        string pdfFile;
        string pptFile;
        string jpgFileDeleted;
        string pdfFileDeleted;
        string pptFileDeleted;

        #endregion

        #region Async Loading

        /// <summary>
        /// Get the PPs using Take.
        /// </summary>
        private async void GetPpAsync(ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            await using var db = new EntityFrameworkContext();
            var tempList = await db.Pp.Include(p => p.Ph).Include(p => p.Ps).Include(p => p.Pk).Include(p => p.Pl)
                .OrderBy(p => p.PAD).ToListAsync();
            ppPagedList = Paginate(tempList, int.TryParse(QtEntriesPerPageInput.Text, out var count) ? count : 0)
                .ToList();
            ppExportPagedList = Paginate(tempList, int.TryParse(QtEntriesPerPageInput.Text, out var count2) ? count2 : 0)
                .ToList();
            AdjustGuiWhenPaging();
            await db.SaveChangesAsync();
        }
        /*private async void GetPpExpAsync(DataGrid dataGrid)
        {
            dataGrid.ItemsSource = null;
            await using var db = new EntityFrameworkContext();
            var tempList = await db.Pp.Include(p => p.Ph).Include(p => p.Ps).Include(p => p.Pk).Include(p => p.Pl)
                .OrderBy(p => p.PAD).ToListAsync();
            ppPagedList = Paginate(tempList, int.TryParse(QtEntriesPerPageInput.Text, out var count) ? count : 0)
                .ToList();
            ppExportPagedList = Paginate(tempList, int.TryParse(QtEntriesPerPageInput.Text, out var count2) ? count2 : 0)
                .ToList();           
            AdjustGuiWhenPagingExport(dataGrid);
            await db.SaveChangesAsync();
        }*/

        /// <summary>
        /// Get the PHs using Take.
        /// </summary>
        private async void GetPhAsync(ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            await using var db = new EntityFrameworkContext();
            dataGrid.ItemsSource = await db.Ph.OrderBy(p => p.PAD).ToListAsync();
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Get the PLs using Take.
        /// </summary>
        private async void GetPlAsync(ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            await using var db = new EntityFrameworkContext();
            dataGrid.ItemsSource = await db.Pl.OrderBy(p => p.PAD).ToListAsync();
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Get the PKs using Take.
        /// </summary>
        private async void GetPkAsync(ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            await using var db = new EntityFrameworkContext();
            dataGrid.ItemsSource = await db.Pk.OrderBy(p => p.PAD).ToListAsync();
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Get the PSs using Take.
        /// </summary>
        /// <param name="dataGrid"></param>
        private async void GetPsAsync(ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            await using var db = new EntityFrameworkContext();
            dataGrid.ItemsSource = await db.Ps.OrderBy(p => p.PAD).ToListAsync();
            await db.SaveChangesAsync();
        }

        #endregion

        #region Load to DataGrid

        /// <summary>
        /// Load PPs to a DataGrid.
        /// </summary>
        /// <param name="pps">List of PPs.</param>
        /// <param name="dataGrid">The table to be filled.</param>
        public void LoadPpCollection(List<Pp> pps, ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = pps; //.Take(int.TryParse(QtEntriesPerPageInput.Text, out var count) ? count : 0);
        }

        public List<Pp> LoadPPCollectionFromExport(List<Pp> pps)
        {
            return pps;
        }

        public List<Pl> LoadPlCollectionFromExport(List<Pl> pls)
        {
            return pls;
        }

        public List<Ph> LoadPhCollectionFromExport(List<Ph> phs)
        {
            return phs;
        }

        public List<Ps> LoadPsCollectionFromExport(List<Ps> pss)
        {
            return pss;
        }

        /// <summary>
        /// Load PHs to a DataGrid.
        /// </summary>
        /// <param name="phs">List of PHs.</param>
        /// <param name="dataGrid">The table to be filled.</param>
        public void LoadPhCollection(List<Ph> phs, DataGrid dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = phs; //.Take(int.TryParse(QtEntriesPerPageInput.Text, out var count) ? count : 0);
            // return;
        }

        /// <summary>
        /// Load PKs to a DataGrid.
        /// </summary>
        /// <param name="pks">List of PKs.</param>
        /// <param name="dataGrid">The table to be filled.</param>
        public void LoadPkCollection(List<Pk> pks, ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = pks; //.Take(int.TryParse(QtEntriesPerPageInput.Text, out var count) ? count : 0);
        }

        /// <summary>
        /// Load PLs to a DataGrid.
        /// </summary>
        /// <param name="pls">List of PLs.</param>
        /// <param name="dataGrid">The table to be filled.</param>
        public void LoadPlCollection(List<Pl> pls, ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = pls; // .Take(int.TryParse(QtEntriesPerPageInput.Text, out var count) ? count : 0);
        }

        /// <summary>
        /// Load PSs to a DataGrid.
        /// </summary>
        /// <param name="pss">List of PSs.</param>
        /// <param name="dataGrid">The table to be filled.</param>
        public void LoadPsCollection(List<Ps> pss, ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = pss; //.Take(int.TryParse(QtEntriesPerPageInput.Text, out var count) ? count : 0);
        }

        #endregion

        #region Load current collections

        /// <summary>
        /// Load current PH collection.
        /// </summary>
        /// <param name="dataGrid">The table to be filled.</param>
        private void LoadCurPhCollection(ItemsControl dataGrid, List<Ph> curPhCollection)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = curPhCollection;
        }

        private void LoadCurPkCollection(ItemsControl dataGrid, List<Pk> curPkCollection)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = curPkCollection;
        }

        private void LoadCurPlCollection(ItemsControl dataGrid, List<Pl> curPlCollection)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = curPlCollection;
        }

        private void LoadCurPsCollection(ItemsControl dataGrid, List<Ps> curPsCollection)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = curPsCollection;
        }

        #endregion

        #region Load One Entity

        private void LoadOnePp(List<Pp> selectedPpsInBearbeitungsAnsicht, ItemsControl dataGrid)
        {
            dataGrid.DataContext = null;
            dataGrid.ItemsSource = selectedPpsInBearbeitungsAnsicht;
        }

        private void LoadOnePk(List<Pk> selectedPksInBearbeitungsAnsicht, ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = selectedPksInBearbeitungsAnsicht;
        }

        private void LoadOnePh(List<Ph> selectedPhsInBearbeitungsAnsicht, DataGrid dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = selectedPhsInBearbeitungsAnsicht;
        }

        private void LoadOnePl(List<Pl> selectedPlsInBearbeitungsAnsicht, ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = selectedPlsInBearbeitungsAnsicht;
        }

        private void LoadOnPs(List<Ps> selectedPssInBearbeitungsAnsicht, DataGrid dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = selectedPssInBearbeitungsAnsicht;
        }

        #endregion

        #region Set data instances

        /// <summary>
        /// Set entity collections using PAD.
        /// </summary>
        /// <param name="Pad">The PAD attribute.</param>
        public async void SetEntityCollectionInstanceByPAD(string Pad)
        {
            CurPadInput.Text = Pad;
            curSelectedPpInBearbeitungFormular = await DbGlobal.Pp.Include(p => p.Ph).Include(p => p.Ps)
                .Include(p => p.Pk).Include(p => p.Pl).Where(p => p.PAD == Pad).FirstAsync();
            curPpCollection = await DbGlobal.Pp.Include(p => p.Ph).Include(p => p.Ps).Include(p => p.Pk)
                .Include(p => p.Pl).Where(p => p.PAD == PAD).ToListAsync();
            curPhCollection = await DbGlobal.Ph.Where(a => a.PAD == Pad).ToListAsync();
            curPkCollection = await DbGlobal.Pk.Where(a => a.PAD == Pad).ToListAsync();
            curPlCollection = await DbGlobal.Pl.Where(a => a.PAD == Pad).ToListAsync();
            curPsCollection = await DbGlobal.Ps.Where(a => a.PAD == Pad).ToListAsync();
            await DbGlobal.SaveChangesAsync();
        }

        /// <summary>
        /// Set data instances using a selected row of a DataGrid.
        /// </summary>
        /// <param name="dataGrid">The table in question.</param>
        public async void SetEntityInstancesFromSelectedRow(DataGrid dataGrid)
        {
            selectedPpsInBearbeitungsAnsicht.Clear();
            selectedPhsInBearbeitungsAnsicht.Clear();
            selectedPksInBearbeitungsAnsicht.Clear();
            selectedPlsInBearbeitungsAnsicht.Clear();
            selectedPssInBearbeitungsAnsicht.Clear();
            try
            {
                selectedRow = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                if (!(dataGrid?.Columns[0].GetCellContent(selectedRow ?? throw new InvalidOperationException())?.Parent
                    is DataGridCell cell0)) return;
                PAD = ((TextBlock)cell0.Content).Text;
                curSelectedPp = await DbGlobal.Pp.Include(p => p.Ph).Include(p => p.Ps).Include(p => p.Pk)
                    .Include(p => p.Pl).Where(p => p.PAD == PAD).FirstAsync();
                selectedPpsInBearbeitungsAnsicht.Add(curSelectedPp);
                curPhCollection = await DbGlobal.Ph.Where(a => a.PAD == curSelectedPp.PAD).ToListAsync();
                selectedPhsInBearbeitungsAnsicht.AddRange(curPhCollection);
                curPkCollection = await DbGlobal.Pk.Where(a => a.PAD == curSelectedPp.PAD).ToListAsync();
                selectedPksInBearbeitungsAnsicht.AddRange(curPkCollection);
                curPlCollection = await DbGlobal.Pl.Where(a => a.PAD == curSelectedPp.PAD).ToListAsync();
                selectedPlsInBearbeitungsAnsicht.AddRange(curPlCollection);
                curPsCollection = await DbGlobal.Ps.Where(a => a.PAD == curSelectedPp.PAD).ToListAsync();
                selectedPssInBearbeitungsAnsicht.AddRange(curPsCollection);
                DbGlobal.SaveChanges();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private List<Ph> collectAllPh(Pp pp)
        {
            return DbGlobal.Ph.Where(a => a.PAD == pp.PAD).ToList();
        }

        private List<Pk> collectAllPk(Pp pp)
        {
            return DbGlobal.Pk.Where(a => a.PAD == pp.PAD).ToList();
        }

        private List<Pl> collectAllPl(Pp pp)
        {
            return DbGlobal.Pl.Where(a => a.PAD == pp.PAD).ToList();
        }

        private List<Ps> collectAllPs(Pp pp)
        {
            return DbGlobal.Ps.Where(a => a.PAD == pp.PAD).ToList();
        }

        public void SetEntityInstancesFromSelectedRows(DataGrid dataGrid)
        {
            selectedPpsInBearbeitungsAnsicht.Clear();
            selectedPhsInBearbeitungsAnsicht.Clear();
            selectedPksInBearbeitungsAnsicht.Clear();
            selectedPlsInBearbeitungsAnsicht.Clear();
            selectedPssInBearbeitungsAnsicht.Clear();
            var rowList = dataGrid.SelectedItems;
            switch (TablesComboBox.SelectedIndex)
            {
                case 0:
                    foreach (Pp pp in rowList)
                    {
                        var curPp = DbGlobal.Pp.Include(p => p.Ph).Include(p => p.Ps).Include(p => p.Pk)
                            .Include(p => p.Pl).FirstOrDefault(p => p.PAD == pp.PAD);
                        selectedPpsInBearbeitungsAnsicht.Add(curPp);
                        selectedPhsInBearbeitungsAnsicht.AddRange(collectAllPh(pp));
                        selectedPksInBearbeitungsAnsicht.AddRange(collectAllPk(pp));
                        selectedPlsInBearbeitungsAnsicht.AddRange(collectAllPl(pp));
                        selectedPssInBearbeitungsAnsicht.AddRange(collectAllPs(pp));
                    }

                    break;
                case 1:
                    //selectedPhsInBearbeitungsAnsicht = collectAllPh(pp, selectedPpsInBearbeitungsAnsicht);
                    foreach (Ph ph in rowList)
                    {
                        selectedPhsInBearbeitungsAnsicht.Add(DbGlobal.Ph.FirstOrDefault(a => a.PAD == ph.PAD));
                    }

                    break;
                case 2:
                    foreach (Pk pk in rowList)
                    {
                        selectedPksInBearbeitungsAnsicht.Add(DbGlobal.Pk.FirstOrDefault(a => a.PAD == pk.PAD));
                    }

                    break;
                case 3:
                    foreach (Pl pl in rowList)
                    {
                        selectedPlsInBearbeitungsAnsicht.Add(DbGlobal.Pl.FirstOrDefault(a => a.PAD == pl.PAD));
                    }

                    break;
                case 4:
                    foreach (Ps ps in rowList)
                    {
                        selectedPssInBearbeitungsAnsicht.Add(DbGlobal.Ps.FirstOrDefault(a => a.PAD == ps.PAD));
                    }

                    break;
            }

            DbGlobal.SaveChanges();
        }

        #endregion

        #region Data sheets display

        #region Methods

        /// <summary>
        /// Locate the relevant data sheet paths,
        /// </summary>
        private void FindDataSheetsPath(Pp pp)
        {
            var mainPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "Skizzen");
            var mainPathDelete = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "geloeschteSkizzen");
            jpgFile = Path.Combine(mainPath, "JPG", pp.PAD[..4], $"{pp.PAD}.jpg");
            pdfFile = Path.Combine(mainPath, "PDF", pp.PAD[..4], $"{pp.PAD}.pdf");
            pptFile = Path.Combine(mainPath, "PowerPoint", pp.PAD[..4], $"{pp.PAD}.ppt");
            jpgFileDeleted = Path.Combine(mainPathDelete, "JPG");
            pdfFileDeleted = Path.Combine(mainPathDelete, "PDF");
            pptFileDeleted = Path.Combine(mainPathDelete, "PowerPoint");
        }

        /// <summary>
        /// Convert the PowerPoint slide to a temporary jpeg file.
        /// </summary>
        private void ShowPowerPointSlide()
        {
            var app = new PowerPoint.Application();
            var slide = app.Presentations.Open(pptFile, MsoTriState.msoTrue, MsoTriState.msoTrue, MsoTriState.msoFalse);
            slide.SaveCopyAs(Path.GetTempPath() + "slide.jpg", PowerPoint.PpSaveAsFileType.ppSaveAsJPG,
                MsoTriState.msoCTrue);
        }

        /// <summary>
        /// Show data sheets and the 'traffic light'.
        /// </summary>
        private void LoadDataSheet(Pp pp)
        {
            FindDataSheetsPath(pp);
            PdfDisplay.Visibility = Visibility.Collapsed;
            JpgDisplay.Source = null;
            PdfDisplay.Source = null;
            JpgIndicator.Background = new SolidColorBrush(Colors.Red);
            PdfIndicator.Background = new SolidColorBrush(Colors.Red);
            PptIndicator.Background = new SolidColorBrush(Colors.Red);
            if (File.Exists(jpgFile))
            {
                DataSheetDisplay.Visibility = Visibility.Visible;
                JpgIndicator.Background = new SolidColorBrush(Colors.LimeGreen);
                JpgDisplay.Source = new BitmapImage(new Uri(jpgFile, UriKind.RelativeOrAbsolute));
            }

            if (File.Exists(pdfFile))
            {
                DataSheetDisplay.Visibility = Visibility.Visible;
                PdfIndicator.Background = new SolidColorBrush(Colors.LimeGreen);
                if (!File.Exists(jpgFile)) //only when there is no jpeg
                {
                    PdfDisplay.Visibility = Visibility.Visible;
                    PdfDisplay.Navigate(new Uri(pdfFile, UriKind.RelativeOrAbsolute));
                }
            }

            if (!File.Exists(pptFile)) return;
            DataSheetDisplay.Visibility = Visibility.Visible;
            PptIndicator.Background = new SolidColorBrush(Colors.LimeGreen);
            if (File.Exists(jpgFile) || File.Exists(pdfFile)) return;
            // only when there is no other formats
            ShowPowerPointSlide();
            JpgDisplay.Source =
                new BitmapImage(new Uri(Path.GetTempPath() + @"slide\Folie1.jpg", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Prompt the user to locate data sheet files and add them to the database.
        /// </summary>
        private void AddMissingDataSheet()
        {
            var openFileDialog = new OpenFileDialog { InitialDirectory = "c:\\" };
            if (!File.Exists(jpgFile))
            {
                openFileDialog.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != true) return;
                var newFile = openFileDialog.FileName;
                File.Copy(newFile, jpgFile);
            }

            if (!File.Exists(pdfFile))
            {
                openFileDialog.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != true) return;
                var newFile = openFileDialog.FileName;
                File.Copy(newFile, pdfFile);
            }

            if (!File.Exists(pptFile))
            {
                openFileDialog.Filter = "pptx files (*.pptx)|*.pptx|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;
                if (openFileDialog.ShowDialog() != true) return;
                var newFile = openFileDialog.FileName;
                File.Copy(newFile, pptFile);
            }

            LoadDataSheet(selectedPpsInHauptAnsicht[0]);
        }

        #endregion

        #region Events

        private void JpgIndicator_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!File.Exists(jpgFile)) AddMissingDataSheet();
        }

        private void PdfIndicator_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!File.Exists(pdfFile)) AddMissingDataSheet();
        }

        private void PptIndicator_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!File.Exists(pptFile)) AddMissingDataSheet();
        }

        #endregion

        #endregion

        #region Table control

        #region Paging

        #region Methods
        /// <summary>
        /// Paginate the PP data sets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The original collection.</param>
        /// <param name="pageSize">The quantity of entries per page.</param>
        /// <returns>The paginated collection.</returns>
        private static IEnumerable<IEnumerable<T>> Paginate<T>(List<T> source, int pageSize)
        {
            using var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var currentPage = new List<T>(pageSize) { enumerator.Current };
                while (currentPage.Count < pageSize && enumerator.MoveNext())
                {
                    currentPage.Add(enumerator.Current);
                }

                yield return new ReadOnlyCollection<T>(currentPage);
            }
        }

        /// <summary>
        /// Update related gui elements when paging is executed.
        /// </summary>
        private void AdjustGuiWhenPaging()//
        {
            PageNumberComboBox.Text = "";
            PageNumberComboBox.Text = "1";
            EntryCountTextBlock.Text = ppPagedList.Sum(l => l.Count()).ToString();
            PageNumberComboBox.ItemsSource = Enumerable.Range(1, ppPagedList.Count);
            PageCountTextBlock.Text = ppPagedList.Count.ToString();
        }
        private void PageNumberComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PageRightButton.IsEnabled = PageNumberComboBox.Text != ppPagedList.Count.ToString();
            MorePageRightButton.IsEnabled = PageNumberComboBox.Text != ppPagedList.Count.ToString();
            LastPageRightButton.IsEnabled = PageNumberComboBox.Text != ppPagedList.Count.ToString();
            if (ppPagedList.Count != 0)
            {
                PpTable.ItemsSource = ppPagedList.ElementAt((int.TryParse(PageNumberComboBox.Text, out var page) ? page : 1) - 1);
            }

        }
        #endregion

        #region Events
        private void PageLeftButton_Click(object sender, RoutedEventArgs e)
        {
            PageNumberComboBox.Text = ((int.TryParse(PageNumberComboBox.Text, out var page) ? page : 2) - 1).ToString();
        }

        private void PageRightButton_Click(object sender, RoutedEventArgs e)
        {
            PageNumberComboBox.Text = ((int.TryParse(PageNumberComboBox.Text, out var page) ? page : 0) + 1).ToString();
        }

        private void MorePageLeftButton_Click(object sender, RoutedEventArgs e)
        {
            var pageNum = int.TryParse(PageNumberComboBox.Text, out var page) ? page : 0;
            PageNumberComboBox.Text = pageNum < 6 ? "1" : (pageNum - 5).ToString();
        }

        private void MorePageRightButton_Click(object sender, RoutedEventArgs e)
        {
            var pageNum = int.TryParse(PageNumberComboBox.Text, out var page) ? page : 0;
            PageNumberComboBox.Text = pageNum > (ppPagedList.Count - 5)
                ? ppPagedList.Count.ToString()
                : (pageNum + 5).ToString();
        }

        private void LastPageLeftButton_Click(object sender, RoutedEventArgs e)
        {
            PageNumberComboBox.Text = "1";
        }

        private void LastPageRightButton_Click(object sender, RoutedEventArgs e)
        {
            PageNumberComboBox.Text = ppPagedList.Count.ToString();
        }
        #endregion

        #endregion

        #region Auto-generation

        /// <summary>
        /// Exclude unnecessary headers from being auto generated.
        /// </summary>
        /// <param name="header">The column headers to be avoided.</param>
        /// <returns></returns>
        private static bool CancelGenerationOfUnwantedColumns(string header)
        {
            switch (header)
            {
                case "PadNavigation":
                case "Pk":
                case "Ph":
                case "Ps":
                case "Pl":
                case "LoeschDatum": return true;
            }

            return false;
        }

        private void PpTable_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Cancel = CancelGenerationOfUnwantedColumns(e.Column.Header.ToString());
        }

        private void PkTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Cancel = CancelGenerationOfUnwantedColumns(e.Column.Header.ToString());
        }

        private void PhTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Cancel = CancelGenerationOfUnwantedColumns(e.Column.Header.ToString());
        }

        private void PlTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Cancel = CancelGenerationOfUnwantedColumns(e.Column.Header.ToString());
        }

        private void PsTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Cancel = CancelGenerationOfUnwantedColumns(e.Column.Header.ToString());
        }

        private void TableForEditing_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Cancel = CancelGenerationOfUnwantedColumns(e.Column.Header.ToString());
        }

        private void QtEntriesPerPageConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if ((int.TryParse(QtEntriesPerPageInput.Text, out var count) ? count : 0) == 0) return;
            //GetPpAsync(PpTable);
            FilterPpTable();
        }

        #endregion

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            TableForEditing.ItemsSource = null;
            selectedPpsInBearbeitungsAnsicht.Clear();
            selectedPhsInBearbeitungsAnsicht.Clear();
            selectedPksInBearbeitungsAnsicht.Clear();
            selectedPlsInBearbeitungsAnsicht.Clear();
            selectedPssInBearbeitungsAnsicht.Clear();
            selectedPhsInHauptAnsicht.Clear();
            selectedPksInHauptAnsicht.Clear();
            selectedPlsInHauptAnsicht.Clear();
            selectedPpsInHauptAnsicht.Clear();
            selectedPssInHauptAnsicht.Clear();
            ResetInputsToNull();
        }

        #endregion

        #region Filter

        #region Methods

        /// <summary>
        /// Filter the content in PP table.
        /// </summary>
        private void FilterPpTable()
        {
            PpTable.ItemsSource = null;
            //using var db = new EntityFrameworkContext();
            var filteredPp = DbGlobal.Pp.OrderBy(p => p.PAD).ToList();
            if (!string.IsNullOrEmpty(PadInput.Text)) filteredPp.RemoveAll(a => !a.PAD.StartsWith(PadInput.Text));
            if (StatusSelection.SelectedIndex != -1 && StatusSelection.SelectedIndex != 5)
                filteredPp.RemoveAll(a => a.PArt != StatusSelection.Text);
            if (!string.IsNullOrEmpty(ContractInput.Text))
                filteredPp.RemoveAll(a => !a.PAuftr.StartsWith(ContractInput.Text));
            if (!string.IsNullOrEmpty(SectionInputFrom.Text))
                filteredPp.RemoveAll(a => string.CompareOrdinal(a.Blattschnitt, SectionInputFrom.Text) < 0);
            if (!string.IsNullOrEmpty(SectionInputTo.Text))
                filteredPp.RemoveAll(a => string.CompareOrdinal(a.Blattschnitt, SectionInputTo.Text) > 0);
            if (!string.IsNullOrEmpty(NumberInputFrom.Text))
                filteredPp.RemoveAll(a => a.PunktNr < Convert.ToInt32(NumberInputFrom.Text));
            if (!string.IsNullOrEmpty(NumberInputTo.Text))
                filteredPp.RemoveAll(a => a.PunktNr > Convert.ToInt32(NumberInputTo.Text));
            ppPagedList = Paginate(filteredPp, int.TryParse(QtEntriesPerPageInput.Text, out var count) ? count : 1)
                .ToList();
            AdjustGuiWhenPaging();
        }

        #endregion

        #region Events

        private void PadInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
            if (PadInput.Text.Length > 3 || PadInput.Text == string.Empty) FilterPpTable();
        }

        private void ContractInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
            if (ContractInput.Text.Length > 2 || ContractInput.Text == string.Empty) FilterPpTable();
        }

        private void StatusSelection_DropDownClosed(object sender, EventArgs e)
        {
            if (_skipEvent) return;
            FilterPpTable();
        }

        private void SectionInputFrom_TextChanged(object sender, RoutedEventArgs e)
        {
            if (_skipEvent) return;
            if (Regex.IsMatch(SectionInputFrom.Text, @"\d{4}[A-Z]{2}") || SectionInputFrom.Text == string.Empty)
                FilterPpTable();
        }

        private void SectionInputTo_TextChanged(object sender, RoutedEventArgs e)
        {
            if (_skipEvent) return;
            if (Regex.IsMatch(SectionInputTo.Text, @"\d{4}[A-Z]{2}") || SectionInputTo.Text == string.Empty)
                FilterPpTable();
        }

        private void NumberFilterConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (_skipEvent) return;
            if (int.TryParse(NumberInputFrom.Text, out _) || NumberInputFrom.Text == string.Empty ||
                int.TryParse(NumberInputTo.Text, out _) || NumberInputTo.Text == string.Empty) FilterPpTable();
        }

        private void LineInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
            if (Regex.IsMatch(SectionInputFrom.Text, @"\d{4}"));
        }

        private void KmInputFrom_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
        }

        private void KmInputTo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
           
        }

        private void FilterResetButton_Click(object sender, RoutedEventArgs e)
        {
            _skipEvent = true;
            PadInput.Clear();
            StatusSelection.SelectedIndex = -1;
            ContractInput.Clear();
            SectionInputFrom.Clear();
            SectionInputTo.Clear();
            NumberInputFrom.Clear();
            NumberInputTo.Clear();
            LineInput.Clear();
            KmInputFrom.Clear();
            KmInputTo.Clear();
            GetPpAsync(PpTable);
        }

        #endregion

        #endregion

        #region Edit

        /// <summary>
        /// Edit all attributes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MassEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (TableForEditing.SelectedItems.Count >= 1)
            {
                switch (TablesComboBox.SelectedIndex)
                {
                    case 0:
                        modifiedPpList.Clear();
                        foreach (var pp in selectedPpsInBearbeitungsAnsicht)
                        {
                            var px = DbGlobal.Pp.FirstOrDefault(p => p.PAD == pp.PAD);
                            selectedPpsInHauptAnsicht.Remove(
                                selectedPpsInHauptAnsicht.FirstOrDefault(a => a.PAD == px.PAD));
                            var pptemp = new Pp();
                            pptemp = EditPpMass(pp, AttributeEditRow01.NewInput.Text, AttributeEditRow02.NewInput.Text,
                                AttributeEditRow03.NewInput.Text, AttributeEditRow04.NewInput.Text,
                                AttributeEditRow05.NewInput.Text, AttributeEditRow06.NewInput.Text,
                                AttributeEditRow07.NewInput.Text, AttributeEditRow08.NewInput.Text,
                                AttributeEditRow09.NewInput.Text, AttributeEditRow10.NewInput.Text);
                            modifiedPpList.Add(pptemp);
                        }

                        DbGlobal.SaveChanges();
                        var lt = new List<Pp>();
                        lt.AddRange(modifiedPpList);
                        lt.AddRange(selectedPpsInHauptAnsicht);
                        selectedPpsInHauptAnsicht.AddRange(modifiedPpList);
                        LoadPpCollection(lt, TableForEditing);
                        break;
                    case 1:
                        modifiedPhList.Clear();
                        foreach (var ph in selectedPhsInBearbeitungsAnsicht)
                        {
                            var pp = DbGlobal.Pp.FirstOrDefault(h => h.PAD == ph.PAD);
                            pp.Ph.Remove(ph);
                            DbGlobal.Ph.Remove(ph);
                            DbGlobal.SaveChanges();
                            selectedPhsInHauptAnsicht.Remove(
                                selectedPhsInHauptAnsicht.FirstOrDefault(a => a.PAD == pp.PAD));
                            var phtemp = new Ph();
                            phtemp = EditPhMass(ph, AttributeEditRow01.NewInput.Text, AttributeEditRow02.NewInput.Text,
                                AttributeEditRow03.NewInput.Text, AttributeEditRow04.NewInput.Text,
                                AttributeEditRow05.NewInput.Text, AttributeEditRow06.NewInput.Text,
                                AttributeEditRow07.NewInput.Text, AttributeEditRow08.NewInput.Text,
                                AttributeEditRow09.NewInput.Text, AttributeEditRow10.NewInput.Text,
                                AttributeEditRow11.NewInput.Text);
                            modifiedPhList.Add(phtemp);
                            pp.Ph.Add(phtemp);
                            DbGlobal.Ph.Add(phtemp);
                        }

                        DbGlobal.SaveChanges();
                        var lh = new List<Ph>();
                        lh.AddRange(modifiedPhList);
                        lh.AddRange(selectedPhsInHauptAnsicht);
                        selectedPhsInHauptAnsicht.AddRange(modifiedPhList);
                        LoadPhCollection(lh, TableForEditing);
                        break;
                    case 2:
                        modifiedPkList.Clear();
                        foreach (var pk in selectedPksInBearbeitungsAnsicht)
                        {
                            var pp = DbGlobal.Pp.FirstOrDefault(k => k.PAD == pk.PAD);
                            pp.Pk.Remove(pk);
                            DbGlobal.Pk.Remove(pk);
                            DbGlobal.SaveChanges();
                            selectedPksInHauptAnsicht.Remove(
                                selectedPksInHauptAnsicht.FirstOrDefault(a => a.PAD == pp.PAD));
                            var pktemp = new Pk();
                            pktemp = EditPkMass(pk, AttributeEditRow01.NewInput.Text, AttributeEditRow02.NewInput.Text,
                                AttributeEditRow03.NewInput.Text, AttributeEditRow04.NewInput.Text,
                                AttributeEditRow05.NewInput.Text, AttributeEditRow06.NewInput.Text,
                                AttributeEditRow07.NewInput.Text, AttributeEditRow08.NewInput.Text,
                                AttributeEditRow09.NewInput.Text, AttributeEditRow10.NewInput.Text,
                                AttributeEditRow11.NewInput.Text, AttributeEditRow12.NewInput.Text,
                                AttributeEditRow13.NewInput.Text);
                            modifiedPkList.Add(pktemp);
                            pp.Pk.Add(pktemp);
                            DbGlobal.Pk.Add(pktemp);
                        }

                        DbGlobal.SaveChanges();
                        var lk = new List<Pk>();
                        lk.AddRange(modifiedPkList);
                        lk.AddRange(selectedPksInHauptAnsicht);
                        selectedPksInHauptAnsicht.AddRange(modifiedPkList);
                        LoadPkCollection(lk, TableForEditing);
                        break;
                    case 3:
                        modifiedPlList.Clear();
                        foreach (var pl in selectedPlsInBearbeitungsAnsicht)
                        {
                            var pp = DbGlobal.Pp.FirstOrDefault(l => l.PAD == pl.PAD);
                            pp.Pl.Remove(pl);
                            DbGlobal.Pl.Remove(pl);
                            DbGlobal.SaveChanges();
                            selectedPlsInHauptAnsicht.Remove(
                                selectedPlsInHauptAnsicht.FirstOrDefault(a => a.PAD == pp.PAD));
                            var pltemp = new Pl();
                            pltemp = EditPlMass(pl, AttributeEditRow01.NewInput.Text, AttributeEditRow02.NewInput.Text,
                                AttributeEditRow03.NewInput.Text, AttributeEditRow04.NewInput.Text,
                                AttributeEditRow05.NewInput.Text, AttributeEditRow06.NewInput.Text,
                                AttributeEditRow07.NewInput.Text, AttributeEditRow08.NewInput.Text,
                                AttributeEditRow09.NewInput.Text, AttributeEditRow10.NewInput.Text,
                                AttributeEditRow11.NewInput.Text, AttributeEditRow12.NewInput.Text);
                            modifiedPlList.Add(pltemp);
                            pp.Pl.Add(pltemp);
                            DbGlobal.Pl.Add(pltemp);
                        }

                        DbGlobal.SaveChanges();
                        var ll = new List<Pl>();
                        ll.AddRange(modifiedPlList);
                        ll.AddRange(selectedPlsInHauptAnsicht);
                        selectedPlsInHauptAnsicht.AddRange(modifiedPlList);
                        LoadPlCollection(ll, TableForEditing);
                        break;
                    case 4:
                        modifiedPsList.Clear();
                        foreach (var ps in selectedPssInBearbeitungsAnsicht)
                        {
                            var pp = DbGlobal.Pp.Where(s => s.PAD == ps.PAD).FirstOrDefault();
                            pp.Ps.Remove(ps);
                            DbGlobal.Ps.Remove(ps);
                            DbGlobal.SaveChanges();
                            selectedPssInHauptAnsicht.Remove(selectedPssInHauptAnsicht.Where(a => a.PAD == pp.PAD)
                                .FirstOrDefault());
                            Ps pstemp = new Ps();
                            pstemp = EditPsMass(ps, AttributeEditRow01.NewInput.Text, AttributeEditRow02.NewInput.Text,
                                AttributeEditRow03.NewInput.Text, AttributeEditRow04.NewInput.Text);
                            modifiedPsList.Add(pstemp);
                            pp.Ps.Add(pstemp);
                            DbGlobal.Ps.Add(pstemp);
                        }

                        DbGlobal.SaveChanges();
                        var ls = new List<Ps>();
                        ls.AddRange(modifiedPsList);
                        ls.AddRange(selectedPssInHauptAnsicht);
                        selectedPssInHauptAnsicht.AddRange(modifiedPsList);
                        LoadPsCollection(ls, TableForEditing);
                        break;
                }
            }
            else
            {
                MessageBox.Show("Bitte Datensatz auswählen!");
            }
        }

        /// <summary>
        /// Edit all PS's attributes.
        /// </summary>
        /// <param name="ps">The PS entry to be edited.</param>
        /// <param name="PStrecke"></param>
        /// <param name="PSTRRiKz"></param>
        /// <param name="Station"></param>
        /// <param name="SDatum"></param>
        /// <returns></returns>
        private Ps EditPsMass(Ps ps, string PStrecke, string PSTRRiKz, string Station, string SDatum)
        {
            try
            {
                if (!string.IsNullOrEmpty(PStrecke)) ps.PStrecke = PStrecke;
                if (!string.IsNullOrEmpty(PSTRRiKz))
                    ps.PSTRRiKz = short.TryParse(PSTRRiKz, out var pstrrikz) ? pstrrikz : ps.PSTRRiKz;
                if (!string.IsNullOrEmpty(Station))
                    ps.Station = double.TryParse(Station, out var station) ? station : ps.Station;
                if (!string.IsNullOrEmpty(SDatum))
                    ps.SDatum = DateTime.TryParse(SDatum, out var sDatum) ? sDatum : ps.SDatum;
            }
            catch (Exception s)
            {
                MessageBox.Show(s.Message);
            }

            return ps;
        }

        /// <summary>
        /// Edit all PSHs attributes.
        /// </summary>
        /// <param name="ph">The PH entry to be edited.</param>
        /// <param name="HStat"></param>
        /// <param name="HSys"></param>
        /// <param name="HFremd"></param>
        /// <param name="H"></param>
        /// <param name="MH"></param>
        /// <param name="MHEXP"></param>
        /// <param name="HDatum"></param>
        /// <param name="HBearb"></param>
        /// <param name="HAuftr"></param>
        /// <param name="HProg"></param>
        /// <param name="HText"></param>
        /// <returns></returns>
        private Ph EditPhMass(Ph ph, string HStat, string HSys, string HFremd, string H, string MH, string MHEXP,
            string HDatum, string HBearb, string HAuftr, string HProg, string HText)
        {
            try
            {
                if (!string.IsNullOrEmpty(HStat)) ph.HStat = HStat;
                if (!string.IsNullOrEmpty(HSys)) ph.HSys = HSys;
                if (!string.IsNullOrEmpty(HFremd)) ph.HFremd = HFremd;
                if (!string.IsNullOrEmpty(H)) ph.H = int.TryParse(H, out var h) ? h : 0;
                if (!string.IsNullOrEmpty(MH)) ph.MH = short.TryParse(MH, out var mh) ? mh : ph.MH;
                if (!string.IsNullOrEmpty(MHEXP)) ph.MHEXP = short.TryParse(MHEXP, out var mhexp) ? mhexp : ph.MHEXP;
                if (!string.IsNullOrEmpty(HDatum)) ph.HDatum = HDatum;
                if (!string.IsNullOrEmpty(HBearb)) ph.HBearb = HBearb;
                if (!string.IsNullOrEmpty(HAuftr)) ph.HAuftr = HAuftr;
                if (!string.IsNullOrEmpty(HProg)) ph.HProg = HProg;
                if (!string.IsNullOrEmpty(HText)) ph.HText = HText;
            }
            catch (Exception h)
            {
                MessageBox.Show(h.Message);
            }

            return ph;
        }

        /// <summary>
        /// Edit all PP's attributes.
        /// </summary>
        /// <param name="pp">The PP entry to be edited.</param>
        /// <param name="PArt"></param>
        /// <param name="Blattschnitt"></param>
        /// <param name="PunktNr"></param>
        /// <param name="PAuftr"></param>
        /// <param name="PProg"></param>
        /// <param name="VermArt"></param>
        /// <param name="Stabil"></param>
        /// <param name="PDatum"></param>
        /// <param name="PBearb"></param>
        /// <param name="PText"></param>
        /// <returns></returns>
        private Pp EditPpMass(Pp pp, string PArt, string Blattschnitt, string PunktNr, string PAuftr, string PProg,
            string VermArt, string Stabil, string PDatum, string PBearb, string PText)
        {
            try
            {
                if (!string.IsNullOrEmpty(PArt)) pp.PArt = PArt;
                if (!string.IsNullOrEmpty(Blattschnitt)) pp.Blattschnitt = Blattschnitt;
                if (!string.IsNullOrEmpty(PunktNr.ToString()))
                    pp.PunktNr = int.TryParse(PunktNr.ToString(), out var ptNr) ? ptNr : 0;
                if (!string.IsNullOrEmpty(PAuftr)) pp.PAuftr = PAuftr;
                if (!string.IsNullOrEmpty(PProg)) pp.PProg = PProg;
                if (!string.IsNullOrEmpty(VermArt.ToString()))
                    pp.VermArt = short.TryParse(VermArt.ToString(), out var vArt) ? vArt : pp.VermArt;
                if (!string.IsNullOrEmpty(Stabil.ToString()))
                    pp.Stabil = short.TryParse(Stabil.ToString(), out var stabil) ? stabil : pp.Stabil;
                if (!string.IsNullOrEmpty(PDatum)) pp.PDatum = PDatum;
                if (!string.IsNullOrEmpty(PBearb)) pp.PBearb = PBearb;
                if (!string.IsNullOrEmpty(PText)) pp.PText = PText;
            }
            catch (Exception p)
            {
                MessageBox.Show(p.Message);
            }

            return pp;
        }

        /// <summary>
        /// Edit all PK's attributes.
        /// </summary>
        /// <param name="pk">The PK entry to be edited.</param>
        /// <param name="KStat"></param>
        /// <param name="KSys"></param>
        /// <param name="HFremd"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        /// <param name="KBearb"></param>
        /// <param name="LProg"></param>
        /// <param name="LAuftr"></param>
        /// <param name="MP"></param>
        /// <param name="MPEXP"></param>
        /// <param name="KText"></param>
        /// <param name="KDatum"></param>
        /// <returns></returns>
        private Pk EditPkMass(Pk pk, string KStat, string KSys, string HFremd, string X, string Y, string Z,
            string KBearb, string LProg, string LAuftr, string MP, string MPEXP, string KText, string KDatum)
        {
            try
            {
                if (!string.IsNullOrEmpty(KStat)) pk.KStat = KStat;
                if (!string.IsNullOrEmpty(KSys)) pk.KSys = KSys;
                if (!string.IsNullOrEmpty(HFremd)) pk.HFremd = HFremd;
                if (!string.IsNullOrEmpty(X)) pk.X = X;
                if (!string.IsNullOrEmpty(Y)) pk.Y = Y;
                if (!string.IsNullOrEmpty(Z)) pk.Z = Z;
                if (!string.IsNullOrEmpty(KBearb)) pk.KBearb = KBearb;
                if (!string.IsNullOrEmpty(LProg)) pk.LProg = LProg;
                if (!string.IsNullOrEmpty(LAuftr)) pk.LAuftr = LAuftr;
                if (!string.IsNullOrEmpty(MP)) pk.MP = MP;
                if (!string.IsNullOrEmpty(MPEXP)) pk.MPEXP = MPEXP;
                if (!string.IsNullOrEmpty(KText)) pk.KText = KText;
                if (!string.IsNullOrEmpty(KDatum))
                    pk.KDatum = DateTime.TryParse(KDatum, out var kDatum) ? kDatum : pk.KDatum;
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }

            return pk;
        }

        /// <summary>
        /// Edit all PL's attributes.
        /// </summary>
        /// <param name="pl">The PL entry to be edited.</param>
        /// <param name="LStat"></param>
        /// <param name="LSys"></param>
        /// <param name="LFremd"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="MP"></param>
        /// <param name="MPEXP"></param>
        /// <param name="LDatum"></param>
        /// <param name="LBearb"></param>
        /// <param name="LAuftr"></param>
        /// <param name="LProg"></param>
        /// <param name="LText"></param>
        /// <returns></returns>
        private Pl EditPlMass(Pl pl, string LStat, string LSys, string LFremd, string X, string Y, string MP,
            string MPEXP, string LDatum, string LBearb, string LAuftr, string LProg, string LText)
        {
            try
            {
                if (!string.IsNullOrEmpty(LStat)) pl.LStat = LStat;
                if (!string.IsNullOrEmpty(LSys)) pl.LSys = LSys;
                if (!string.IsNullOrEmpty(LFremd)) pl.LFremd = LFremd;
                if (!string.IsNullOrEmpty(X)) pl.X = double.TryParse(Y, out var x) ? x : pl.X;
                if (!string.IsNullOrEmpty(Y)) pl.Y = double.TryParse(Y, out var y) ? y : pl.X;
                if (!string.IsNullOrEmpty(MP)) pl.MP = short.TryParse(MP, out var mp) ? mp : pl.MP;
                if (!string.IsNullOrEmpty(MPEXP)) pl.MPEXP = short.TryParse(MPEXP, out var mpexp) ? mpexp : pl.MPEXP;
                if (!string.IsNullOrEmpty(LDatum)) pl.LDatum = LDatum;
                if (!string.IsNullOrEmpty(LBearb)) pl.LBearb = LBearb;
                if (!string.IsNullOrEmpty(LAuftr)) pl.LAuftr = LAuftr;
                if (!string.IsNullOrEmpty(LProg)) pl.LProg = LProg;
                if (!string.IsNullOrEmpty(LText)) pl.LText = LText;
            }
            catch (Exception l)
            {
                MessageBox.Show(l.Message);
            }

            return pl;
        }

        /// <summary>
        /// Edit PAD.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PadEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewPadInput.Text != string.Empty)
            {
                //detachedPpList = detacheKindsFromParentPp(selectedPpsInBearbeitungsAnsicht);
                if (TableForEditing.SelectedItems.Count == 1)
                {
                    try
                    {
                        switch (TablesComboBox.SelectedIndex)
                        {
                            case 0:
                                {
                                    modifiedPpList.Clear();
                                    foreach (var pp in selectedPpsInBearbeitungsAnsicht)
                                    {
                                        var _p = removeKids(pp);
                                        selectedPpsInHauptAnsicht.Remove(selectedPpsInHauptAnsicht.FirstOrDefault(a => a.PAD == _p.PAD));
                                        _p.PAD = NewPadInput.Text;
                                        //var px = DbGlobal.Pp.FirstOrDefault(p => p.PAD == _p.PAD);
                                        //selectedPpsInHauptAnsicht.Remove(
                                        //    selectedPpsInHauptAnsicht.FirstOrDefault(a => a.PAD == _p.PAD));
                                        DbGlobal.Pp.Add(_p);
                                        DbGlobal.SaveChanges();
                                        if (PlCheckBox.IsChecked == true)
                                        {
                                            foreach (var pl in detachedPlList)
                                            {
                                                pl.PAD = NewPadInput.Text;
                                                _p.Pl.Add(pl);
                                            }
                                        }
                                        else
                                        {
                                            foreach (var pl in detachedPlList)
                                            {
                                                DbGlobal.Pl.Add(pl);
                                            }
                                        }

                                        if (PhCheckBox.IsChecked == true)
                                        {
                                            foreach (var ph in detachedPhList)
                                            {
                                                ph.PAD = NewPadInput.Text;
                                                _p.Ph.Add(ph);
                                            }
                                        }
                                        else
                                        {
                                            foreach (var ph in detachedPhList)
                                            {
                                                DbGlobal.Ph.Add(ph);
                                            }
                                        }

                                        if (PsCheckBox.IsChecked == true)
                                        {
                                            foreach (var ps in detachedPsList)
                                            {
                                                ps.PAD = NewPadInput.Text;
                                                _p.Ps.Add(ps);
                                            }
                                        }
                                        else
                                        {
                                            foreach (var ps in detachedPsList)
                                            {
                                                DbGlobal.Ps.Add(ps);
                                            }
                                        }

                                        if (PkCheckBox.IsChecked == true)
                                        {
                                            foreach (var pk in detachedPkList)
                                            {
                                                pk.PAD = NewPadInput.Text;
                                                _p.Pk.Add(pk);
                                            }
                                        }
                                        else
                                        {
                                            foreach (var pk in detachedPkList)
                                            {
                                                DbGlobal.Pk.Add(pk);
                                            }
                                        }

                                        modifiedPpList.Add(_p);
                                        selectedPpsInHauptAnsicht.Add(_p);
                                        //DbGlobal.Pp.Add(_p);
                                        DbGlobal.SaveChanges();
                                    }

                                    LoadPpCollection(selectedPpsInHauptAnsicht, TableForEditing);
                                    break;
                                }
                            case 1:
                                {
                                    MessageBox.Show("bitte zu PP Tabelle wechsel !");
                                    break;
                                }
                            case 2:
                                {
                                    MessageBox.Show("bitte zu PP Tabelle wechsel !");
                                    break;
                                }
                            case 3:
                                {
                                    MessageBox.Show("bitte zu PP Tabelle wechsel !");
                                    break;
                                }
                            case 4:
                                {
                                    MessageBox.Show("bitte zu PP Tabelle wechsel !");
                                    break;
                                }
                        }
                    }
                    catch (Exception ev)
                    {

                        MessageBox.Show(ev.InnerException.Message);

                    }
                }
                else if (TableForEditing.Items.Count > 0 && TableForEditing.SelectedItems.Count == 0)
                {
                    var dialogResult = MessageBox.Show("mehere PPs können keine gleichen PAD haben", "Achtung !",
                        button: MessageBoxButton.YesNo);
                    switch (dialogResult)
                    {
                        case MessageBoxResult.Yes:
                            MessageBox.Show("bitte Datensatz auswählen");
                            break;
                        case MessageBoxResult.No: return;
                    }
                }
                else if (TableForEditing.SelectedItems.Count > 1)
                {
                    MessageBox.Show("Bitte nur einen Datensatz auswählen!");
                }
                else
                {
                    MessageBox.Show("Bitte Datensatz auswählen!");
                }
            }
            else
            {
                MessageBox.Show("Neuer Wert eingeben !");
            }
        }

        #endregion

        #region Delete

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pp"></param>
        private void ConvertPpToDeletedPp(Pp pp)
        {
            DbGlobal.GeloeschtPp.Add(new GeloeschtPp()
            {
                PAD = pp.PAD,
                PArt = pp.PArt,
                PAuftr = pp.PAuftr,
                PBearb = pp.PBearb,
                PDatum = pp.PDatum,
                PProg = pp.PProg,
                PText = pp.PText,
                LoeschDatum = DateTime.Now,
                Import = pp.Import,
                Stabil = pp.Stabil,
                VermArt = pp.VermArt,
                PunktNr = pp.PunktNr,
                Blattschnitt = pp.Blattschnitt
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ph"></param>
        private void ConvertPhToDeletedPh(Ph ph)
        {
            DbGlobal.GeloeschtPh.AddRange(new GeloeschtPh
            {
                PAD = ph.PAD,
                HProg = ph.HProg,
                MHEXP = ph.MHEXP,
                H = ph.H,
                HAuftr = ph.HAuftr,
                HBearb = ph.HBearb,
                HDatum = ph.HDatum,
                HFremd = ph.HFremd,
                HStat = ph.HStat,
                HSys = ph.HSys,
                HText = ph.HText,
                MH = ph.MH,
                LoeschDatum = DateTime.Now,
                Import = ph.Import,
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pl"></param>
        private void ConvertPlToDeletedPl(Pl pl)
        {
            DbGlobal.GeloeschtPl.AddRange(new GeloeschtPl()
            {
                PAD = pl.PAD,
                LStat = pl.LStat,
                LProg = pl.LProg,
                LAuftr = pl.LAuftr,
                LBearb = pl.LBearb,
                LDatum = pl.LDatum,
                LFremd = pl.LFremd,
                LSys = pl.LSys,
                LText = pl.LText,
                X = pl.X,
                Y = pl.Y,
                LoeschDatum = DateTime.Now,
                Import = pl.Import,
                MP = pl.MP,
                MPEXP = pl.MPEXP
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pk"></param>
        private void ConvertPkToDeletedPk(Pk pk)
        {
            DbGlobal.GeloeschtPk.AddRange(new GeloeschtPk()
            {
                PAD = pk.PAD,
                LProg = pk.LProg,
                MP = pk.MP,
                MPEXP = pk.MPEXP,
                HFremd = pk.HFremd,
                KBearb = pk.KBearb,
                KDatum = pk.KDatum,
                KStat = pk.KStat,
                KSys = pk.KSys,
                KText = pk.KText,
                Import = pk.Import,
                LAuftr = pk.LAuftr,
                X = pk.X,
                Y = pk.Y,
                Z = pk.Z,
                LoeschDatum = DateTime.Now
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ps"></param>
        private void ConvertPsToDeletedPs(Ps ps)
        {
            DbGlobal.GeloeschtPs.AddRange(new GeloeschtPs()
            {
                PAD = ps.PAD,
                PStrecke = ps.PStrecke,
                PSTRRiKz = ps.PSTRRiKz,
                Import = ps.Import,
                LoeschDatum = DateTime.Now,
                SDatum = ps.SDatum,
                Station = ps.Station
            });
        }

        /// <summary>
        /// Delete data set.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DelButton_Click(object sender, RoutedEventArgs e)
        {
            if (TableForEditing.SelectedItems.Count >= 1)
            {
                switch (TablesComboBox.SelectedIndex)
                {
                    case 0:
                        {
                            foreach (var pp in selectedPpsInBearbeitungsAnsicht)
                            {
                                Pp __p = new Pp();
                                __p = removeKids(pp);
                                if (PlDelCheckBox.IsChecked == true)
                                {
                                    if (LSysDelCheckBox1.IsChecked == true)
                                    {
                                        //var a = detachedPlList.Where(c => c.LSys == "DR0").ToList();
                                        foreach (var pl in detachedPlList.Where(c => c.LSys == "DR0").ToList())
                                        {
                                            DbGlobal.GeloeschtPl.AddRange(new GeloeschtPl()
                                            {
                                                PAD = pl.PAD,
                                                LStat = pl.LStat,
                                                LProg = pl.LProg,
                                                LAuftr = pl.LAuftr,
                                                LBearb = pl.LBearb,
                                                LDatum = pl.LDatum,
                                                LFremd = pl.LFremd,
                                                LSys = pl.LSys,
                                                LText = pl.LText,
                                                X = pl.X,
                                                Y = pl.Y,
                                                LoeschDatum = DateTime.Now,
                                                Import = pl.Import,
                                                MP = pl.MP,
                                                MPEXP = pl.MPEXP
                                            });
                                            selectedPlsInHauptAnsicht.Remove(selectedPlsInHauptAnsicht
                                                .Where(a => a.LSys == "ER0").FirstOrDefault());
                                        }

                                        DbGlobal.SaveChanges();
                                    }
                                    else if (LSysDelCheckBox2.IsChecked == true)
                                    {
                                        //var a = detachedPlList.Where(c => c.LSys == "ER0").ToList();
                                        foreach (var pl in detachedPlList.Where(c => c.LSys == "ER0").ToList())
                                        {
                                            DbGlobal.GeloeschtPl.AddRange(new GeloeschtPl()
                                            {
                                                PAD = pl.PAD,
                                                LStat = pl.LStat,
                                                LProg = pl.LProg,
                                                LAuftr = pl.LAuftr,
                                                LBearb = pl.LBearb,
                                                LDatum = pl.LDatum,
                                                LFremd = pl.LFremd,
                                                LSys = pl.LSys,
                                                LText = pl.LText,
                                                X = pl.X,
                                                Y = pl.Y,
                                                LoeschDatum = DateTime.Now,
                                                Import = pl.Import,
                                                MP = pl.MP,
                                                MPEXP = pl.MPEXP
                                            });
                                            selectedPlsInHauptAnsicht.Remove(selectedPlsInHauptAnsicht
                                                .Where(a => a.LSys == "ER0").FirstOrDefault());
                                        }

                                        DbGlobal.SaveChanges();
                                    }
                                    else if (LSysDelCheckBox3.IsChecked == true)
                                    {
                                        //var a = detachedPlList.Where(c => c.LSys == "FR0").ToList();
                                        foreach (var pl in detachedPlList.Where(c => c.LSys == "FR0").ToList())
                                        {
                                            DbGlobal.GeloeschtPl.AddRange(new GeloeschtPl()
                                            {
                                                PAD = pl.PAD,
                                                LStat = pl.LStat,
                                                LProg = pl.LProg,
                                                LAuftr = pl.LAuftr,
                                                LBearb = pl.LBearb,
                                                LDatum = pl.LDatum,
                                                LFremd = pl.LFremd,
                                                LSys = pl.LSys,
                                                LText = pl.LText,
                                                X = pl.X,
                                                Y = pl.Y,
                                                LoeschDatum = DateTime.Now,
                                                Import = pl.Import,
                                                MP = pl.MP,
                                                MPEXP = pl.MPEXP
                                            });
                                            selectedPlsInHauptAnsicht.Remove(selectedPlsInHauptAnsicht
                                                .Where(a => a.LSys == "FR0").FirstOrDefault());
                                        }

                                        DbGlobal.SaveChanges();
                                    }
                                    else
                                    {
                                        foreach (var pl in detachedPlList)
                                        {
                                            DbGlobal.GeloeschtPl.AddRange(new GeloeschtPl()
                                            {
                                                PAD = pl.PAD,
                                                LStat = pl.LStat,
                                                LProg = pl.LProg,
                                                LAuftr = pl.LAuftr,
                                                LBearb = pl.LBearb,
                                                LDatum = pl.LDatum,
                                                LFremd = pl.LFremd,
                                                LSys = pl.LSys,
                                                LText = pl.LText,
                                                X = pl.X,
                                                Y = pl.Y,
                                                LoeschDatum = DateTime.Now,
                                                Import = pl.Import,
                                                MP = pl.MP,
                                                MPEXP = pl.MPEXP
                                            });
                                            selectedPlsInHauptAnsicht.Remove(selectedPlsInHauptAnsicht
                                                .Where(a => a.PAD == pp.PAD).FirstOrDefault());
                                        }

                                        DbGlobal.SaveChanges();
                                    }
                                }
                                else
                                {
                                    foreach (var pl in detachedPlList)
                                    {
                                        DbGlobal.Pl.Add(pl);
                                        DbGlobal.SaveChanges();
                                    }
                                }

                                if (PhDelCheckBox.IsChecked == true)
                                {
                                    if (detachedPhList.Count != 0)
                                    {
                                        if (HSysDelCheckBox1.IsChecked == true)
                                        {
                                            //var a = detachedPhList.Where(c => c.HSys == "O00").ToList();
                                            foreach (var ph in detachedPhList.Where(c => c.HSys == "O00").ToList())
                                            {
                                                DbGlobal.GeloeschtPh.AddRange(new GeloeschtPh
                                                {
                                                    PAD = ph.PAD,
                                                    HProg = ph.HProg,
                                                    MHEXP = ph.MHEXP,
                                                    H = ph.H,
                                                    HAuftr = ph.HAuftr,
                                                    HBearb = ph.HBearb,
                                                    HDatum = ph.HDatum,
                                                    HFremd = ph.HFremd,
                                                    HStat = ph.HStat,
                                                    HSys = ph.HSys,
                                                    HText = ph.HText,
                                                    MH = ph.MH,
                                                    LoeschDatum = DateTime.Now,
                                                    Import = ph.Import,
                                                });
                                                selectedPhsInHauptAnsicht.Remove(selectedPhsInHauptAnsicht
                                                    .Where(a => a.HSys == "O00").FirstOrDefault());
                                            }

                                            DbGlobal.SaveChanges();
                                        }
                                        else if (HSysDelCheckBox2.IsChecked == true)
                                        {
                                            //var a = detachedPhList.Where(c => c.HSys == "R00").ToList();
                                            foreach (var ph in detachedPhList.Where(c => c.HSys == "R00").ToList())
                                            {
                                                DbGlobal.GeloeschtPh.AddRange(new GeloeschtPh
                                                {
                                                    PAD = ph.PAD,
                                                    HProg = ph.HProg,
                                                    MHEXP = ph.MHEXP,
                                                    H = ph.H,
                                                    HAuftr = ph.HAuftr,
                                                    HBearb = ph.HBearb,
                                                    HDatum = ph.HDatum,
                                                    HFremd = ph.HFremd,
                                                    HStat = ph.HStat,
                                                    HSys = ph.HSys,
                                                    HText = ph.HText,
                                                    MH = ph.MH,
                                                    LoeschDatum = DateTime.Now,
                                                    Import = ph.Import,
                                                });
                                                selectedPhsInHauptAnsicht.Remove(selectedPhsInHauptAnsicht
                                                    .Where(a => a.HSys == "R00").FirstOrDefault());
                                            }

                                            DbGlobal.SaveChanges();
                                        }
                                        else
                                        {
                                            foreach (var ph in detachedPhList)
                                            {
                                                DbGlobal.GeloeschtPh.AddRange(new GeloeschtPh
                                                {
                                                    PAD = ph.PAD,
                                                    HProg = ph.HProg,
                                                    MHEXP = ph.MHEXP,
                                                    H = ph.H,
                                                    HAuftr = ph.HAuftr,
                                                    HBearb = ph.HBearb,
                                                    HDatum = ph.HDatum,
                                                    HFremd = ph.HFremd,
                                                    HStat = ph.HStat,
                                                    HSys = ph.HSys,
                                                    HText = ph.HText,
                                                    MH = ph.MH,
                                                    LoeschDatum = DateTime.Now,
                                                    Import = ph.Import,
                                                });
                                                selectedPhsInHauptAnsicht.Remove(selectedPhsInHauptAnsicht
                                                    .Where(a => a.PAD == pp.PAD).FirstOrDefault());
                                            }

                                            DbGlobal.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var ph in detachedPhList)
                                    {
                                        DbGlobal.Ph.Add(ph);
                                        DbGlobal.SaveChanges();
                                    }
                                }

                                if (PkDelCheckBox.IsChecked == true)
                                {
                                    if (detachedPkList.Count != 0)
                                    {
                                        if (KSysDelCheckBox.IsChecked == true)
                                        {
                                            //var a = detachedPkList.Where(c => c.KSys == "Y01").ToList();
                                            foreach (var pk in detachedPkList.Where(c => c.KSys == "Y01").ToList())
                                            {
                                                DbGlobal.GeloeschtPk.AddRange(new GeloeschtPk()
                                                {
                                                    PAD = pk.PAD,
                                                    LProg = pk.LProg,
                                                    MP = pk.MP,
                                                    MPEXP = pk.MPEXP,
                                                    HFremd = pk.HFremd,
                                                    KBearb = pk.KBearb,
                                                    KDatum = pk.KDatum,
                                                    KStat = pk.KStat,
                                                    KSys = pk.KSys,
                                                    KText = pk.KText,
                                                    Import = pk.Import,
                                                    LAuftr = pk.LAuftr,
                                                    X = pk.X,
                                                    Y = pk.Y,
                                                    Z = pk.Z,
                                                    LoeschDatum = DateTime.Now
                                                });
                                                selectedPksInHauptAnsicht.Remove(selectedPksInHauptAnsicht
                                                    .Where(a => a.KSys == "Y01").FirstOrDefault());
                                            }

                                            DbGlobal.SaveChanges();
                                        }
                                        else
                                        {
                                            foreach (var pk in detachedPkList)
                                            {
                                                DbGlobal.GeloeschtPk.AddRange(new GeloeschtPk()
                                                {
                                                    PAD = pk.PAD,
                                                    LProg = pk.LProg,
                                                    MP = pk.MP,
                                                    MPEXP = pk.MPEXP,
                                                    HFremd = pk.HFremd,
                                                    KBearb = pk.KBearb,
                                                    KDatum = pk.KDatum,
                                                    KStat = pk.KStat,
                                                    KSys = pk.KSys,
                                                    KText = pk.KText,
                                                    Import = pk.Import,
                                                    LAuftr = pk.LAuftr,
                                                    X = pk.X,
                                                    Y = pk.Y,
                                                    Z = pk.Z,
                                                    LoeschDatum = DateTime.Now
                                                });
                                                selectedPksInHauptAnsicht.Remove(selectedPksInHauptAnsicht
                                                    .Where(a => a.PAD == pp.PAD).FirstOrDefault());
                                            }

                                            DbGlobal.SaveChanges();
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var pk in detachedPkList)
                                    {
                                        DbGlobal.Pk.Add(pk);
                                        DbGlobal.SaveChanges();
                                    }
                                }

                                if (PsDelCheckBox.IsChecked == true)
                                {
                                    if (detachedPsList.Count != 0)
                                    {
                                        foreach (var ps in detachedPsList)
                                        {
                                            DbGlobal.GeloeschtPs.AddRange(new GeloeschtPs()
                                            {
                                                PAD = ps.PAD,
                                                PStrecke = ps.PStrecke,
                                                PSTRRiKz = ps.PSTRRiKz,
                                                Import = ps.Import,
                                                LoeschDatum = DateTime.Now,
                                                SDatum = ps.SDatum,
                                                Station = ps.Station
                                            });
                                            selectedPssInHauptAnsicht.Remove(selectedPssInHauptAnsicht
                                                .Where(a => a.PAD == pp.PAD).FirstOrDefault());
                                        }

                                        DbGlobal.SaveChanges();
                                    }
                                }
                                else
                                {
                                    foreach (var ps in detachedPsList)
                                    {
                                        DbGlobal.Ps.Add(ps);
                                        DbGlobal.SaveChanges();
                                    }
                                }

                                if (DataSheetDelCheckBox.IsChecked == true)
                                {
                                    foreach (var pp_ in detachedPpList)
                                    {
                                        FindDataSheetsPath(pp_);
                                        if (File.Exists(pptFile)) // PowerPoint
                                        {
                                            try
                                            {
                                                File.Copy(pptFile, pptFileDeleted, true);
                                                File.Delete(pptFile);
                                            }
                                            catch (IOException io)
                                            {
                                                MessageBox.Show(io.Message);
                                            }
                                        }

                                        if (File.Exists(pdfFile)) // PDF
                                        {
                                            try
                                            {
                                                File.Copy(pdfFile, pdfFileDeleted, true);
                                                File.Delete(pdfFile);
                                            }
                                            catch (IOException io)
                                            {
                                                MessageBox.Show(io.Message);
                                            }
                                        }

                                        if (File.Exists(jpgFile)) // JPG
                                        {
                                            try
                                            {
                                                File.Copy(jpgFile, jpgFileDeleted, true);
                                                File.Delete(jpgFile);
                                            }
                                            catch (IOException io)
                                            {
                                                MessageBox.Show(io.Message);
                                            }
                                        }
                                    }
                                }

                                DbGlobal.GeloeschtPp.Add(new GeloeschtPp()
                                {
                                    PAD = __p.PAD,
                                    PArt = __p.PArt,
                                    PAuftr = __p.PAuftr,
                                    PBearb = __p.PBearb,
                                    PDatum = __p.PDatum,
                                    PProg = __p.PProg,
                                    PText = __p.PText,
                                    LoeschDatum = DateTime.Now,
                                    Import = __p.Import,
                                    Stabil = __p.Stabil,
                                    VermArt = __p.VermArt,
                                    PunktNr = __p.PunktNr,
                                    Blattschnitt = __p.Blattschnitt
                                });
                                selectedPpsInHauptAnsicht.Remove(selectedPpsInHauptAnsicht.Where(a => a.PAD == pp.PAD)
                                    .FirstOrDefault());
                                DbGlobal.SaveChanges();
                            }

                            LoadPpCollection(selectedPpsInHauptAnsicht, TableForEditing);
                            MessageBox.Show("Pp(s) erfolgreich gelöscht!");
                            break;
                        }
                    case 1:
                        {
                            foreach (Ph ph in selectedPhsInBearbeitungsAnsicht)
                            {
                                var pn = DbGlobal.Pp.Where(h => h.PAD == ph.PAD).FirstOrDefault();
                                DbGlobal.GeloeschtPh.Add(new GeloeschtPh
                                {
                                    PAD = ph.PAD,
                                    HProg = ph.HProg,
                                    MHEXP = ph.MHEXP,
                                    H = ph.H,
                                    HAuftr = ph.HAuftr,
                                    HBearb = ph.HBearb,
                                    HDatum = ph.HDatum,
                                    HFremd = ph.HFremd,
                                    HStat = ph.HStat,
                                    HSys = ph.HSys,
                                    HText = ph.HText,
                                    MH = ph.MH,
                                    LoeschDatum = DateTime.Now,
                                    Import = ph.Import,
                                });
                                selectedPhsInHauptAnsicht.Remove(selectedPhsInHauptAnsicht.Where(a => a.PAD == pn.PAD)
                                    .FirstOrDefault());
                                pn.Ph.Remove(ph);
                                DbGlobal.Ph.Remove(ph);
                                DbGlobal.SaveChanges();
                            }

                            LoadPhCollection(selectedPhsInHauptAnsicht, TableForEditing);
                            MessageBox.Show("Ph(s) erfolgreich gelöscht!");
                            break;
                        }
                    case 2:
                        foreach (Pk pk in selectedPksInBearbeitungsAnsicht)
                        {
                            var pm = DbGlobal.Pp.Where(k => k.PAD == pk.PAD).FirstOrDefault();
                            DbGlobal.GeloeschtPk.AddRange(new GeloeschtPk()
                            {
                                PAD = pk.PAD,
                                LProg = pk.LProg,
                                MP = pk.MP,
                                MPEXP = pk.MPEXP,
                                HFremd = pk.HFremd,
                                KBearb = pk.KBearb,
                                KDatum = pk.KDatum,
                                KStat = pk.KStat,
                                KSys = pk.KSys,
                                KText = pk.KText,
                                Import = pk.Import,
                                LAuftr = pk.LAuftr,
                                X = pk.X,
                                Y = pk.Y,
                                Z = pk.Z,
                                LoeschDatum = DateTime.Now
                            });
                            selectedPksInHauptAnsicht.Remove(selectedPksInHauptAnsicht.Where(a => a.PAD == pm.PAD)
                                .FirstOrDefault());
                            pm.Pk.Remove(pk);
                            DbGlobal.Pk.Remove(pk);
                            DbGlobal.SaveChanges();
                        }

                        LoadPkCollection(selectedPksInHauptAnsicht, TableForEditing);
                        MessageBox.Show("Pk(s) erfolgreich gelöscht!");
                        break;
                    case 3:
                        foreach (Pl pl in selectedPlsInBearbeitungsAnsicht)
                        {
                            var px = DbGlobal.Pp.Where(l => l.PAD == pl.PAD).FirstOrDefault();
                            DbGlobal.GeloeschtPl.AddRange(new GeloeschtPl()
                            {
                                PAD = pl.PAD,
                                LStat = pl.LStat,
                                LProg = pl.LProg,
                                LAuftr = pl.LAuftr,
                                LBearb = pl.LBearb,
                                LDatum = pl.LDatum,
                                LFremd = pl.LFremd,
                                LSys = pl.LSys,
                                LText = pl.LText,
                                X = pl.X,
                                Y = pl.Y,
                                LoeschDatum = DateTime.Now,
                                Import = pl.Import,
                                MP = pl.MP,
                                MPEXP = pl.MPEXP
                            });
                            selectedPlsInHauptAnsicht.Remove(selectedPlsInHauptAnsicht.Where(a => a.PAD == px.PAD)
                                .FirstOrDefault());
                            px.Pl.Remove(pl);
                            DbGlobal.Pl.Remove(pl);
                            DbGlobal.SaveChanges();
                        }

                        LoadPlCollection(selectedPlsInHauptAnsicht, TableForEditing);
                        MessageBox.Show("Pl(s) erfolgreich gelöscht!");
                        break;
                    case 4:
                        foreach (Ps ps in selectedPssInBearbeitungsAnsicht)
                        {
                            var pe = DbGlobal.Pp.Where(l => l.PAD == ps.PAD).FirstOrDefault();
                            DbGlobal.GeloeschtPs.AddRange(new GeloeschtPs()
                            {
                                PAD = ps.PAD,
                                PStrecke = ps.PStrecke,
                                PSTRRiKz = ps.PSTRRiKz,
                                Import = ps.Import,
                                LoeschDatum = DateTime.Now,
                                SDatum = ps.SDatum,
                                Station = ps.Station
                            });
                            selectedPssInHauptAnsicht.Remove(selectedPssInHauptAnsicht.Where(s => s.PAD == pe.PAD)
                                .FirstOrDefault());
                            pe.Ps.Remove(ps);
                            DbGlobal.Ps.Remove(ps);
                            DbGlobal.SaveChanges();
                        }

                        LoadPsCollection(selectedPssInHauptAnsicht, TableForEditing);
                        MessageBox.Show("Ps(s) erfolgreich gelöscht!");
                        break;
                }
            }
            else if (TableForEditing.SelectedItems.Count == 0)
            {
                MessageBoxResult dialogResult =
                    MessageBox.Show("Löschung bestätigen", "Achtung !", button: MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    switch (TablesComboBox.SelectedIndex)
                    {
                        case 0:
                            foreach (Pp pp in selectedPpsInBearbeitungsAnsicht)
                            {
                                DbGlobal.Pp.Remove(pp);
                            }

                            MessageBox.Show("Pp(s) erfolgreich gelöscht!");
                            break;
                        case 1:
                            foreach (Ph ph in selectedPhsInBearbeitungsAnsicht)
                            {
                                DbGlobal.Ph.Remove(ph);
                            }

                            MessageBox.Show("Ph(s) erfolgreich gelöscht!");
                            break;
                        case 2:
                            foreach (Pk pk in selectedPksInBearbeitungsAnsicht)
                            {
                                DbGlobal.Pk.Remove(pk);
                            }

                            MessageBox.Show("Pk(s) erfolgreich gelöscht!");
                            break;
                        case 3:
                            foreach (Pl pl in selectedPlsInBearbeitungsAnsicht)
                            {
                                DbGlobal.Pl.Remove(pl);
                            }

                            MessageBox.Show("Pl(s) erfolgreich gelöscht!");
                            break;
                        case 4:
                            foreach (Ps ps in selectedPssInBearbeitungsAnsicht)
                            {
                                DbGlobal.Ps.Remove(ps);
                            }

                            MessageBox.Show("Ps(s) erfolgreich gelöscht!");
                            break;
                    }

                    TableForEditing.ItemsSource = null;
                }
                else if (dialogResult == MessageBoxResult.No)
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("Bitte Datensatz auswahlen !");
            }
        }

        #endregion

        #region Reset

        /// <summary>
        /// Set Inputs to  null
        /// </summary>
        public void ResetInputsToNull()
        {
            CurPadInput.Text = string.Empty;
            AttributeEditRow01.CurInput.Text = string.Empty;
            AttributeEditRow02.CurInput.Text = string.Empty;
            AttributeEditRow03.CurInput.Text = string.Empty;
            AttributeEditRow04.CurInput.Text = string.Empty;
            AttributeEditRow05.CurInput.Text = string.Empty;
            AttributeEditRow06.CurInput.Text = string.Empty;
            AttributeEditRow07.CurInput.Text = string.Empty;
            AttributeEditRow08.CurInput.Text = string.Empty;
            AttributeEditRow09.CurInput.Text = string.Empty;
            AttributeEditRow10.CurInput.Text = string.Empty;
            AttributeEditRow11.CurInput.Text = string.Empty;
            AttributeEditRow12.CurInput.Text = string.Empty;
            AttributeEditRow13.CurInput.Text = string.Empty;
        }

        /// <summary>
        /// Reset the inputs.
        /// </summary>
        /// 
        public void ResetInputOnTableForEditing()
        {
            try
            {
                switch (TablesComboBox.SelectedIndex)
                {
                    case 0:
                        CurPadInput.Text = selectedPpsInBearbeitungsAnsicht[0].PAD;
                        AttributeEditRow01.CurInput.Text = selectedPpsInBearbeitungsAnsicht[0].PArt;
                        AttributeEditRow02.CurInput.Text = selectedPpsInBearbeitungsAnsicht[0].Blattschnitt;
                        AttributeEditRow03.CurInput.Text = selectedPpsInBearbeitungsAnsicht[0].PunktNr.ToString();
                        AttributeEditRow04.CurInput.Text = selectedPpsInBearbeitungsAnsicht[0].PAuftr;
                        AttributeEditRow05.CurInput.Text = selectedPpsInBearbeitungsAnsicht[0].PProg;
                        AttributeEditRow06.CurInput.Text = selectedPpsInBearbeitungsAnsicht[0].VermArt.ToString();
                        AttributeEditRow07.CurInput.Text = selectedPpsInBearbeitungsAnsicht[0].Stabil.ToString();
                        AttributeEditRow08.CurInput.Text = selectedPpsInBearbeitungsAnsicht[0].PDatum;
                        AttributeEditRow09.CurInput.Text = selectedPpsInBearbeitungsAnsicht[0].PBearb;
                        AttributeEditRow10.CurInput.Text = selectedPpsInBearbeitungsAnsicht[0].PText;
                        //AttributeEditRow11.CurInput.Text = curSelectedPpInBearbeitungFormular.Import.ToString();
                        break;
                    case 1:
                        CurPadInput.Text = selectedPhsInBearbeitungsAnsicht[0].PAD;
                        AttributeEditRow01.CurInput.Text = selectedPhsInBearbeitungsAnsicht[0].HStat;
                        AttributeEditRow02.CurInput.Text = selectedPhsInBearbeitungsAnsicht[0].HSys;
                        AttributeEditRow03.CurInput.Text = selectedPhsInBearbeitungsAnsicht[0].HFremd;
                        AttributeEditRow04.CurInput.Text = selectedPhsInBearbeitungsAnsicht[0].H.ToString();
                        AttributeEditRow05.CurInput.Text = selectedPhsInBearbeitungsAnsicht[0].MH.ToString();
                        AttributeEditRow06.CurInput.Text = selectedPhsInBearbeitungsAnsicht[0].MHEXP.ToString();
                        AttributeEditRow07.CurInput.Text = selectedPhsInBearbeitungsAnsicht[0].HDatum;
                        AttributeEditRow08.CurInput.Text = selectedPhsInBearbeitungsAnsicht[0].HBearb;
                        AttributeEditRow09.CurInput.Text = selectedPhsInBearbeitungsAnsicht[0].HAuftr;
                        AttributeEditRow10.CurInput.Text = selectedPhsInBearbeitungsAnsicht[0].HProg;
                        AttributeEditRow11.CurInput.Text = selectedPhsInBearbeitungsAnsicht[0].HText;
                        //AttributeEditRow12.CurInput.Text = curSelectedPhInBearbeitungFormular.Import.ToString();                   
                        break;
                    case 2:
                        CurPadInput.Text = selectedPksInBearbeitungsAnsicht[0].PAD;
                        AttributeEditRow01.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].KStat;
                        AttributeEditRow02.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].KSys;
                        AttributeEditRow03.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].HFremd;
                        AttributeEditRow04.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].X;
                        AttributeEditRow05.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].Y;
                        AttributeEditRow06.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].Z;
                        AttributeEditRow07.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].KBearb;
                        AttributeEditRow08.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].LProg;
                        AttributeEditRow09.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].LAuftr;
                        AttributeEditRow10.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].MP;
                        AttributeEditRow11.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].MPEXP;
                        AttributeEditRow12.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].KText;
                        AttributeEditRow13.CurInput.Text = selectedPksInBearbeitungsAnsicht[0].KDatum.ToString();
                        break;
                    case 3:
                        CurPadInput.Text = selectedPlsInBearbeitungsAnsicht[0].PAD;
                        AttributeEditRow01.CurInput.Text = selectedPlsInBearbeitungsAnsicht[0].LStat;
                        AttributeEditRow02.CurInput.Text = selectedPlsInBearbeitungsAnsicht[0].LSys;
                        AttributeEditRow03.CurInput.Text = selectedPlsInBearbeitungsAnsicht[0].LFremd;
                        AttributeEditRow04.CurInput.Text = selectedPlsInBearbeitungsAnsicht[0].X.ToString();
                        AttributeEditRow05.CurInput.Text = selectedPlsInBearbeitungsAnsicht[0].Y.ToString();
                        AttributeEditRow06.CurInput.Text = selectedPlsInBearbeitungsAnsicht[0].MP.ToString();
                        AttributeEditRow07.CurInput.Text = selectedPlsInBearbeitungsAnsicht[0].MPEXP.ToString();
                        AttributeEditRow08.CurInput.Text = selectedPlsInBearbeitungsAnsicht[0].LDatum;
                        AttributeEditRow09.CurInput.Text = selectedPlsInBearbeitungsAnsicht[0].LBearb;
                        AttributeEditRow10.CurInput.Text = selectedPlsInBearbeitungsAnsicht[0].LAuftr;
                        AttributeEditRow11.CurInput.Text = selectedPlsInBearbeitungsAnsicht[0].LProg;
                        AttributeEditRow12.CurInput.Text = selectedPlsInBearbeitungsAnsicht[0].LText;
                        //AttributeEditRow13.CurInput.Text = curSelectedPlInBearbeitungFormular.Import.ToString();
                        break;
                    case 4:
                        CurPadInput.Text = selectedPssInBearbeitungsAnsicht[0].PAD;
                        AttributeEditRow01.CurInput.Text = selectedPssInBearbeitungsAnsicht[0].PStrecke;
                        AttributeEditRow02.CurInput.Text = selectedPssInBearbeitungsAnsicht[0].PSTRRiKz.ToString();
                        AttributeEditRow03.CurInput.Text = selectedPssInBearbeitungsAnsicht[0].Station.ToString();
                        AttributeEditRow04.CurInput.Text = selectedPssInBearbeitungsAnsicht[0].SDatum.ToString();
                        AttributeEditRow05.CurInput.Text = selectedPssInBearbeitungsAnsicht[0].Import.ToString();
                        break;
                }
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.Message);
            }

            NewPadInput.Text = string.Empty;
            //ResetGuiLabels();
        }

        /// <summary>
        /// Reset the Labels.
        /// </summary>
        public void ResetLabelsOnTableForEditing()
        {
            PhTableCbxItem.Visibility = Visibility.Visible;
            PkTableCbxItem.Visibility = Visibility.Visible;
            PlTableCbxItem.Visibility = Visibility.Visible;
            PsTableCbxItem.Visibility = Visibility.Visible;
            if (!string.IsNullOrEmpty(PAD))
            {
                if (curSelectedPp.Ph.Count == 0) PhTableCbxItem.Visibility = Visibility.Collapsed;
                if (curSelectedPp.Pk.Count == 0) PkTableCbxItem.Visibility = Visibility.Collapsed;
                if (curSelectedPp.Pl.Count == 0) PlTableCbxItem.Visibility = Visibility.Collapsed;
                if (curSelectedPp.Ps.Count == 0) PsTableCbxItem.Visibility = Visibility.Collapsed;
            }

            PadEditCheckBoxRow.IsEnabled = false;
            PhCheckBox.IsChecked = false;
            PkCheckBox.IsChecked = false;
            PlCheckBox.IsChecked = false;
            PsCheckBox.IsChecked = false;
            AttributeEditRow06.Visibility = Visibility.Visible;
            AttributeEditRow07.Visibility = Visibility.Visible;
            AttributeEditRow08.Visibility = Visibility.Visible;
            AttributeEditRow09.Visibility = Visibility.Visible;
            AttributeEditRow10.Visibility = Visibility.Visible;
            AttributeEditRow11.Visibility = Visibility.Visible;
            AttributeEditRow12.Visibility = Visibility.Visible;
            AttributeEditRow13.Visibility = Visibility.Visible;
            DelCheckBoxRow1.IsEnabled = false;
            HSysDelCheckBox1.IsChecked = false;
            HSysDelCheckBox2.IsChecked = false;
            KSysDelCheckBox.IsChecked = false;
            LSysDelCheckBox1.IsChecked = false;
            LSysDelCheckBox2.IsChecked = false;
            LSysDelCheckBox3.IsChecked = false;
            DataSheetDelCheckBox.IsEnabled = false;
            DataSheetDelCheckBox.IsChecked = false;
            switch (TablesComboBox.SelectedIndex)
            {
                case 0:
                    PadEditCheckBoxRow.IsEnabled = true;
                    DelCheckBoxRow1.IsEnabled = true;
                    AttributeEditRow01.Title = "PArt";
                    AttributeEditRow02.Title = "Blattschnitt";
                    AttributeEditRow03.Title = "PunktNr";
                    AttributeEditRow04.Title = "PAuftr";
                    AttributeEditRow05.Title = "PProg";
                    AttributeEditRow06.Title = "VermArt";
                    AttributeEditRow07.Title = "Stabil";
                    AttributeEditRow08.Title = "PDatum";
                    AttributeEditRow09.Title = "PBearb";
                    AttributeEditRow10.Title = "PText";
                    AttributeEditRow11.Visibility = Visibility.Collapsed;
                    AttributeEditRow12.Visibility = Visibility.Collapsed;
                    AttributeEditRow13.Visibility = Visibility.Collapsed;
                    DataSheetDelCheckBox.IsEnabled = true;
                    break;
                case 1:
                    AttributeEditRow01.Title = "HStat";
                    AttributeEditRow02.Title = "HSys";
                    AttributeEditRow03.Title = "HFremd";
                    AttributeEditRow04.Title = "H";
                    AttributeEditRow05.Title = "MH";
                    AttributeEditRow06.Title = "MHEXP";
                    AttributeEditRow07.Title = "HDatum";
                    AttributeEditRow08.Title = "HBearb";
                    AttributeEditRow09.Title = "HAuftr";
                    AttributeEditRow10.Title = "HProg";
                    AttributeEditRow11.Title = "HText";
                    AttributeEditRow12.Visibility = Visibility.Collapsed;
                    AttributeEditRow13.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    AttributeEditRow01.Title = "KStat";
                    AttributeEditRow02.Title = "KSys";
                    AttributeEditRow03.Title = "HFremd";
                    AttributeEditRow04.Title = "X";
                    AttributeEditRow05.Title = "Y";
                    AttributeEditRow06.Title = "Z";
                    AttributeEditRow07.Title = "KBearb";
                    AttributeEditRow08.Title = "LProg";
                    AttributeEditRow09.Title = "LAuftr";
                    AttributeEditRow10.Title = "MP";
                    AttributeEditRow11.Title = "MPEXP";
                    AttributeEditRow12.Title = "KText";
                    AttributeEditRow13.Title = "KDatum";
                    break;
                case 3:
                    AttributeEditRow01.Title = "LStat";
                    AttributeEditRow02.Title = "LSys";
                    AttributeEditRow03.Title = "LFremd";
                    AttributeEditRow04.Title = "X";
                    AttributeEditRow05.Title = "Y";
                    AttributeEditRow06.Title = "MP";
                    AttributeEditRow07.Title = "MPEXP";
                    AttributeEditRow08.Title = "LDatum";
                    AttributeEditRow09.Title = "LBearb";
                    AttributeEditRow10.Title = "LAuftr";
                    AttributeEditRow11.Title = "LProg";
                    AttributeEditRow12.Title = "LText";
                    AttributeEditRow13.Visibility = Visibility.Collapsed;
                    break;
                case 4:
                    AttributeEditRow01.Title = "PStrecke";
                    AttributeEditRow02.Title = "PSTRRiKz";
                    AttributeEditRow03.Title = "Station";
                    AttributeEditRow04.Title = "SDatum";
                    AttributeEditRow06.Visibility = Visibility.Collapsed;
                    AttributeEditRow06.Visibility = Visibility.Collapsed;
                    AttributeEditRow07.Visibility = Visibility.Collapsed;
                    AttributeEditRow08.Visibility = Visibility.Collapsed;
                    AttributeEditRow09.Visibility = Visibility.Collapsed;
                    AttributeEditRow10.Visibility = Visibility.Collapsed;
                    AttributeEditRow11.Visibility = Visibility.Collapsed;
                    AttributeEditRow12.Visibility = Visibility.Collapsed;
                    AttributeEditRow13.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        #endregion

        #region Tab control

        public void PpTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPpsInHauptAnsicht.Clear();
            selectedPhsInHauptAnsicht.Clear();
            selectedPlsInHauptAnsicht.Clear();
            selectedPssInHauptAnsicht.Clear();
            selectedPksInHauptAnsicht.Clear();
            TrafficLight.Visibility = Visibility.Visible;
            PpTable = sender as DataGrid;
            if (PpTable != null && PpTable.SelectedItems.Count > 1)
            {
                foreach (Pp pp in PpTable.SelectedItems)
                {
                    var phs = DbGlobal.Ph.Where(p => p.PAD == pp.PAD).ToList();
                    var pls = DbGlobal.Pl.Where(p => p.PAD == pp.PAD).ToList();
                    var pss = DbGlobal.Ps.Where(p => p.PAD == pp.PAD).ToList();
                    var pks = DbGlobal.Pk.Where(p => p.PAD == pp.PAD).ToList();
                    selectedPpsInHauptAnsicht.Add(pp);
                    selectedPhsInHauptAnsicht.AddRange(phs);
                    selectedPlsInHauptAnsicht.AddRange(pls);
                    selectedPssInHauptAnsicht.AddRange(pss);
                    selectedPksInHauptAnsicht.AddRange(pks);
                }

                if (selectedPpsInHauptAnsicht.Count == 1)
                {
                    LoadDataSheet(selectedPpsInHauptAnsicht[0]);
                    LoadCurPhCollection(PhTable, selectedPhsInHauptAnsicht);
                    LoadCurPkCollection(PkTable, selectedPksInHauptAnsicht);
                    LoadCurPlCollection(PlTable, selectedPlsInHauptAnsicht);
                    LoadCurPsCollection(PsTable, selectedPssInHauptAnsicht);
                }
                else if (selectedPpsInHauptAnsicht.Count > 1)
                {
                    PhTable.ItemsSource = null;
                    PkTable.ItemsSource = null;
                    PsTable.ItemsSource = null;
                    PlTable.ItemsSource = null;
                }
            }
            else
            {
                try
                {
                    curSelectedPpInBearbeitungFormular = DbGlobal.Pp.Include(p => p.Ph).Include(p => p.Ps)
                        .Include(p => p.Pk).Include(p => p.Pl).FirstOrDefault(p => p == (Pp)PpTable.SelectedItem);
                    curPhCollection = DbGlobal.Ph.Where(a => a.PAD == curSelectedPpInBearbeitungFormular.PAD).ToList();
                    curPkCollection = DbGlobal.Pk.Where(a => a.PAD == curSelectedPpInBearbeitungFormular.PAD).ToList();
                    curPlCollection = DbGlobal.Pl.Where(a => a.PAD == curSelectedPpInBearbeitungFormular.PAD).ToList();
                    curPsCollection = DbGlobal.Ps.Where(a => a.PAD == curSelectedPpInBearbeitungFormular.PAD).ToList();
                    DbGlobal.SaveChanges();
                    selectedPpsInHauptAnsicht.Add(curSelectedPpInBearbeitungFormular);
                    selectedPhsInHauptAnsicht.AddRange(curPhCollection);
                    selectedPlsInHauptAnsicht.AddRange(curPlCollection);
                    selectedPssInHauptAnsicht.AddRange(curPsCollection);
                    selectedPksInHauptAnsicht.AddRange(curPkCollection);
                    CurPadInput.Text = selectedPpsInHauptAnsicht[0].PAD;
                    LoadDataSheet(selectedPpsInHauptAnsicht[0]);

                    //Load other tables
                    LoadCurPhCollection(PhTable, selectedPhsInHauptAnsicht);
                    LoadCurPkCollection(PkTable, selectedPksInHauptAnsicht);
                    LoadCurPlCollection(PlTable, selectedPlsInHauptAnsicht);
                    LoadCurPsCollection(PsTable, selectedPssInHauptAnsicht);
                }
                catch (Exception exception)
                {
                    // ignored
                }
            }
        }

        private void PhTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TablesComboBox.SelectedIndex = 1;
        }

        private void PlTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TablesComboBox.SelectedIndex = 3;
        }

        private void PkTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TablesComboBox.SelectedIndex = 2;
        }

        private void PsTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TablesComboBox.SelectedIndex = 4;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            selectedPpsInBearbeitungsAnsicht.Clear();
            selectedPhsInBearbeitungsAnsicht.Clear();
            selectedPksInBearbeitungsAnsicht.Clear();
            selectedPlsInBearbeitungsAnsicht.Clear();
            selectedPssInBearbeitungsAnsicht.Clear();
            selectedPpsInHauptAnsicht.Clear();
            selectedPhsInHauptAnsicht.Clear();
            selectedPksInHauptAnsicht.Clear();
            selectedPlsInHauptAnsicht.Clear();
            selectedPssInHauptAnsicht.Clear();
            PpTable.ItemsSource = null;
            PhTable.ItemsSource = null;
            PkTable.ItemsSource = null;
            PlTable.ItemsSource = null;
            PsTable.ItemsSource = null;
            GetPpAsync(PpTable);
            // GetPps(PpTable);
        }

        private void EntryRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (TableForEditing.SelectedItems.Count == 0)
            {
                MessageBox.Show("Bitte Datensatz auswählen !");
            }
            else if (TableForEditing.SelectedItems.Count >= 1)
            {
                switch (TablesComboBox.SelectedIndex)
                {
                    case 0:
                        foreach (Pp pp in selectedPpsInBearbeitungsAnsicht)
                        {
                            var px = DbGlobal.Pp.Where(p => p.PAD == pp.PAD).FirstOrDefault();
                            selectedPpsInHauptAnsicht.Remove(selectedPpsInHauptAnsicht.Where(a => a.PAD == px.PAD)
                                .FirstOrDefault());
                            selectedPhsInHauptAnsicht.Remove(selectedPhsInHauptAnsicht.Where(a => a.PAD == px.PAD)
                                .FirstOrDefault());
                            selectedPksInHauptAnsicht.Remove(selectedPksInHauptAnsicht.Where(a => a.PAD == px.PAD)
                                .FirstOrDefault());
                            selectedPlsInHauptAnsicht.Remove(selectedPlsInHauptAnsicht.Where(a => a.PAD == px.PAD)
                                .FirstOrDefault());
                            selectedPssInHauptAnsicht.Remove(selectedPssInHauptAnsicht.Where(a => a.PAD == px.PAD)
                                .FirstOrDefault());
                        }

                        selectedPpsInBearbeitungsAnsicht.Clear();
                        LoadPpCollection(selectedPpsInHauptAnsicht, TableForEditing);
                        break;
                }
                ResetInputsToNull();
            }
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ReferenceEquals(e.OriginalSource, MainTabControl)) return;



            if (EditTabItem.IsSelected)
            {
                // PpTable = sender as  DataGrid;
                if (PpTable.SelectedItems.Count >= 1)
                {
                    switch (TablesComboBox.SelectedIndex)
                    {
                        case 0:
                            LoadPpCollection(selectedPpsInHauptAnsicht, TableForEditing);
                            break;
                        case 1:
                            LoadPhCollection(selectedPhsInHauptAnsicht, TableForEditing);
                            break;
                        case 2:
                            LoadPkCollection(selectedPksInHauptAnsicht, TableForEditing);
                            break;
                        case 3:
                            LoadPlCollection(selectedPlsInHauptAnsicht, TableForEditing);
                            break;
                        case 4:
                            LoadPsCollection(selectedPssInHauptAnsicht, TableForEditing);
                            break;
                    }
                }
                else
                {
                    switch (TablesComboBox.SelectedIndex)
                    {
                        case 0:
                            GetPpAsync(TableForEditing);
                            break;
                        case 1:
                            GetPhAsync(TableForEditing);
                            break;
                        case 2:
                            GetPkAsync(TableForEditing);
                            break;
                        case 3:
                            GetPlAsync(TableForEditing);
                            break;
                        case 4:
                            GetPsAsync(TableForEditing);
                            break;
                    }
                }

                ResetLabelsOnTableForEditing();
                // ResetInputOnTableForEditing();
            }

            if (ViewTabItem.IsSelected)
            {
                //TablesComboBox.SelectedIndex = 0;
                if (PpTable.SelectedItems != null) return;
            }
        }

        private void TablesComboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (TableForEditing.SelectedItems.Count == 1)
            {
                // if once is Selected on the TableForEditing
                switch (TablesComboBox.SelectedIndex)
                {
                    case 0:
                        LoadOnePp(curPpCollection, TableForEditing);
                        break;
                    case 1:
                        LoadOnePh(curPhCollection, TableForEditing);
                        break;
                    case 2:
                        LoadOnePk(curPkCollection, TableForEditing);
                        break;
                    case 3:
                        LoadOnePl(curPlCollection, TableForEditing);
                        break;
                    case 4:
                        LoadOnPs(curPsCollection, TableForEditing);
                        break;
                }
            }
            else if (TableForEditing.SelectedItems.Count > 1)
            {
                // if more than 1 are selected on the TableForEditing
                switch (TablesComboBox.SelectedIndex)
                {
                    case 0:
                        LoadPpCollection(selectedPpsInBearbeitungsAnsicht, TableForEditing);
                        break;
                    case 1:
                        LoadPhCollection(selectedPhsInBearbeitungsAnsicht, TableForEditing);
                        break;
                    case 2:
                        LoadPkCollection(selectedPksInBearbeitungsAnsicht, TableForEditing);
                        break;
                    case 3:
                        LoadPlCollection(selectedPlsInBearbeitungsAnsicht, TableForEditing);
                        break;
                    case 4:
                        LoadPsCollection(selectedPssInBearbeitungsAnsicht, TableForEditing);
                        break;
                }
            }
            else if (TableForEditing.SelectedItem == null)
            {
                if (PpTable.SelectedItems != null)
                {
                    switch (TablesComboBox.SelectedIndex)
                    {
                        case 0:
                            LoadPpCollection(selectedPpsInHauptAnsicht, TableForEditing);
                            break;
                        case 1:
                            LoadPhCollection(selectedPhsInHauptAnsicht, TableForEditing);
                            break;
                        case 2:
                            LoadPkCollection(selectedPksInHauptAnsicht, TableForEditing);
                            break;
                        case 3:
                            LoadPlCollection(selectedPlsInHauptAnsicht, TableForEditing);
                            break;
                        case 4:
                            LoadPsCollection(selectedPssInHauptAnsicht, TableForEditing);
                            break;
                    }
                }
                else
                {
                    switch (TablesComboBox.SelectedIndex)
                    {
                        case 0:
                            GetPpAsync(TableForEditing);
                            break;
                        case 1:
                            GetPhAsync(TableForEditing);
                            break;
                        case 2:
                            GetPkAsync(TableForEditing);
                            break;
                        case 3:
                            GetPlAsync(TableForEditing);
                            break;
                        case 4:
                            GetPsAsync(TableForEditing);
                            break;
                    }
                }
            }

            ResetInputsToNull();
            ResetLabelsOnTableForEditing();
        }

        /// <summary>
        /// Set data instances on entry selections.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void TableForEditing_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TableForEditing = sender as DataGrid;
            TableForEditingTemp = TableForEditing;
            if (TableForEditing != null && TableForEditing.SelectedItems.Count == 1)
            {
                SetEntityInstancesFromSelectedRow(TableForEditing);
                ResetInputOnTableForEditing();
                ResetLabelsOnTableForEditing();
            }
            else if (
                TableForEditing != null &&
                TableForEditing.SelectedItems.Count >
                1 /*Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)*/)
            {
                SetEntityInstancesFromSelectedRows(TableForEditing);
                ResetInputsToNull();
            }
        }

        #region Remove Kids from  Parent

        public Pp removeKids(Pp pp)
        {
            detachedPhList.Clear();
            detachedPkList.Clear();
            detachedPlList.Clear();
            detachedPsList.Clear();
            foreach (Ph ph in pp.Ph.AsQueryable().ToList())
            {
                detachedPhList.Add(ph);
                pp.Ph.Remove(ph);
                DbGlobal.Ph.Remove(ph);
            }

            foreach (Pk pk in pp.Pk.AsQueryable().ToList())
            {
                detachedPkList.Add(pk);
                pp.Pk.Remove(pk);
                DbGlobal.Pk.Remove(pk);
            }

            foreach (Pl pl in pp.Pl.AsQueryable().ToList())
            {
                detachedPlList.Add(pl);
                pp.Pl.Remove(pl);
                DbGlobal.Pl.Remove(pl);
            }

            foreach (Ps ps in pp.Ps.AsQueryable().ToList())
            {
                detachedPsList.Add(ps);
                pp.Ps.Remove(ps);
                DbGlobal.Ps.Remove(ps);
            }

            DbGlobal.Pp.Remove(pp);
            DbGlobal.SaveChanges();
            return pp;
        }
        #endregion

        #endregion

        #region Import: Data

        #region Import : Fill Temp Tables

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            if (DbGlobal.ImportPp.Count() == 0 && DbGlobal.ImportPh.Count() == 0 && DbGlobal.ImportPk.Count() == 0 &&
                DbGlobal.ImportPl.Count() == 0 && DbGlobal.ImportPs.Count() == 0)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                //openFileDialog.Filter = "All Files |*.xls; *.xlsx; *.csv; *.dbb; *.nap";

                if (openFileDialog.ShowDialog() == true)
                {
                    importTextBox.Text = openFileDialog.FileName;
                }
                if (importTextBox.Text.Contains(".xls") || importTextBox.Text.Contains(".xlsx"))
                {
                    Import.ImportExcelFiles(dataTableforTemp, importTextBox.Text);
                    Import.ImportToTemps(dataTableforTemp, "Excel", DbGlobal);
                }
                else if (importTextBox.Text.Contains(".csv"))
                {
                    Import.ImportCsvFiles(dataTableforTemp, openFileDialog.FileName);
                    Import.ImportToTemps(dataTableforTemp, "CSV", DbGlobal);
                }
                else if (importTextBox.Text.Contains(".dbb"))
                {
                    Import.ImportDbbFiles(dataTableforTemp, openFileDialog.FileName);
                    Import.ImportToTemps(dataTableforTemp, "DBB", DbGlobal);
                }
                else if (importTextBox.Text.Contains(".nap"))
                {
                    Import.ImportNapFiles(dataTableforTemp, openFileDialog.FileName);
                    Import.ImportToTemps(dataTableforTemp, "NAP", DbGlobal);
                }

                ImportTablesComboBox_DropDownClosed(sender, e);
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Aktuell gibt es bereits importierete Daten. Sollen die gelöscht werden?", "ACHTUNG!!!", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    LöschenButton_Click(sender, e);
                }
                ImportTablesComboBox_DropDownClosed(sender, e);
            }
        }

        #endregion

        #region Import : Löschen
        private void LöschenButton_Click(object sender, RoutedEventArgs e)
        {
            Import.ClearTempTables(DbGlobal, dataTableforTemp, importTextBox.Text);

            importDatagrid.ItemsSource = null;
            importDatagrid.Items.Refresh();
            importTextBox.Text = null;
        }
        private void AusgewählteDatensätzeLöschenButton_Click(object sender, RoutedEventArgs e)
        {
            while (importDatagrid.SelectedItems.Count > 0)
            {

                switch (ImportTablesComboBox.SelectedIndex)
                {
                    case 0:
                        Import.DeleteTheSamePADFromOtherTables("ImportPp", importDatagrid.SelectedItems, DbGlobal);
                        break;
                    case 1:
                        Import.DeleteTheSamePADFromOtherTables("ImportPh", importDatagrid.SelectedItems, DbGlobal);
                        break;
                    case 2:
                        Import.DeleteTheSamePADFromOtherTables("ImportPk", importDatagrid.SelectedItems, DbGlobal);
                        break;
                    case 3:
                        Import.DeleteTheSamePADFromOtherTables("ImportPl", importDatagrid.SelectedItems, DbGlobal);
                        break;
                    case 4:
                        Import.DeleteTheSamePADFromOtherTables("ImportPs", importDatagrid.SelectedItems, DbGlobal);
                        break;
                }
                ImportTablesComboBox_DropDownClosed(sender, e);
            }
        }

        #endregion

        #region Import : Load Temps To DataGrid
        private void ImportTablesComboBox_DropDownClosed(object sender, EventArgs e)
        {
            switch (ImportTablesComboBox.SelectedIndex)
            {
                case 0:
                    LoadImportPp(importDatagrid);
                    break;
                case 1:
                    LoadImportPH(importDatagrid);
                    break;
                case 2:
                    LoadImportPK(importDatagrid);
                    break;
                case 3:
                    LoadImportPL(importDatagrid);
                    break;
                case 4:
                    LoadImportPS(importDatagrid);
                    break;
            }

        }
        private void importDatagrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "PAD")
            {
                e.Column.IsReadOnly = true;
            }
        }
        private void LoadImportPp(ItemsControl dataGrid)
        {
            dataGrid.DataContext = null;
            dataGrid.ItemsSource = DbGlobal.ImportPp.ToList();

        }
        private void LoadImportPH(ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = DbGlobal.ImportPh.ToList();
        }
        private void LoadImportPK(ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = DbGlobal.ImportPk.ToList();
        }

        private void LoadImportPL(ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = DbGlobal.ImportPl.ToList();
        }
        private void LoadImportPS(ItemsControl dataGrid)
        {
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = DbGlobal.ImportPs.ToList();
        }


        #endregion

        #region Import : Save Data In Database 
        private Conflict conflict;
        async private void SpeichernButton_Click(object sender, RoutedEventArgs e)
        {
            List<ImportPp> l1 = DbGlobal.ImportPp.ToList();
            List<ImportPh> l2 = DbGlobal.ImportPh.ToList();
            List<ImportPk> l3 = DbGlobal.ImportPk.ToList();
            List<ImportPl> l4 = DbGlobal.ImportPl.ToList();
            List<ImportPs> l5 = DbGlobal.ImportPs.ToList();
            System.Diagnostics.Debug.WriteLine("Coming...");

            List<KeyValuePair<Pp, ImportPp>> conflictsPp = new List<KeyValuePair<Pp, ImportPp>>();
            List<KeyValuePair<Ph, ImportPh>> conflictsPh = new List<KeyValuePair<Ph, ImportPh>>();
            List<KeyValuePair<Pk, ImportPk>> conflictsPk = new List<KeyValuePair<Pk, ImportPk>>();
            List<KeyValuePair<Pl, ImportPl>> conflictsPl = new List<KeyValuePair<Pl, ImportPl>>();
            List<KeyValuePair<Ps, ImportPs>> conflictsPs = new List<KeyValuePair<Ps, ImportPs>>();

            //await using var db = new EntityFrameworkContext();
            List<Ph> dListPh = await DbGlobal.Ph.OrderBy(p => p.PAD).ToListAsync();
            List<Pp> dListPp = await DbGlobal.Pp.OrderBy(p => p.PAD).ToListAsync();
            List<Pk> dListPk = await DbGlobal.Pk.OrderBy(p => p.PAD).ToListAsync();
            List<Pl> dListPl = await DbGlobal.Pl.OrderBy(p => p.PAD).ToListAsync();
            List<Ps> dListPs = await DbGlobal.Ps.OrderBy(p => p.PAD).ToListAsync();

            // checking conflicts Pp
            dListPp.ForEach(delegate (Pp pp)
            {
                l1.ForEach(delegate (ImportPp obj)
                {
                    if (CheckConflict(obj.PAD.ToString(), pp.PAD.ToString()))
                    {
                        conflictsPp.Add(new KeyValuePair<Pp, ImportPp>(pp, obj));
                    }
                });
            });

            // checking conflicts Ph
            dListPh.ForEach(delegate (Ph ph)
            {
                l2.ForEach(delegate (ImportPh obj)
                {
                    if (CheckConflict(obj.PAD.ToString(), ph.PAD.ToString()))
                    {
                        conflictsPh.Add(new KeyValuePair<Ph, ImportPh>(ph, obj));
                    }
                });
            });

            // checking conflicts Pk
            dListPk.ForEach(delegate (Pk pk)
            {
                l3.ForEach(delegate (ImportPk obj)
                {
                    if (CheckConflict(obj.PAD.ToString(), pk.PAD.ToString()))
                    {
                        conflictsPk.Add(new KeyValuePair<Pk, ImportPk>(pk, obj));
                    }
                });
            });

            // checking conflicts Pl
            dListPl.ForEach(delegate (Pl pl)
            {
                l4.ForEach(delegate (ImportPl obj)
                {
                    if (CheckConflict(obj.PAD.ToString(), pl.PAD.ToString()))
                    {
                        conflictsPl.Add(new KeyValuePair<Pl, ImportPl>(pl, obj));
                    }
                });
            });

            // checking conflicts Ps
            dListPs.ForEach(delegate (Ps ps)
            {
                l5.ForEach(delegate (ImportPs obj)
                {
                    if (CheckConflict(obj.PAD.ToString(), ps.PAD.ToString()))
                    {
                        conflictsPs.Add(new KeyValuePair<Ps, ImportPs>(ps, obj));
                    }
                });
            });


            if (conflictsPp.Count > 0 || conflictsPh.Count > 0 || conflictsPk.Count > 0 || conflictsPl.Count > 0 || conflictsPs.Count > 0)
            {
                MessageBox.Show("Wir haben Konflikt gefunden!");
                new Conflict(conflictsPp, conflictsPh, conflictsPk, conflictsPl, conflictsPs).Show();
                itemsSource = importDatagrid.ItemsSource;
                text = importTextBox.Text;
            }
            else
            {
                Import.SaveDataInDataBase(DbGlobal, dataTableforTemp, importTextBox.Text);
                LöschenButton_Click(sender, e);
                MessageBox.Show("Die Datensätze sind gespeichert.", "Info");
            }
        }

        private bool CheckConflict(string pad1, string pad2)
        {
            if (pad1.ToString().Equals(pad2.ToString()))
                return true;
            return false;
        }

        #endregion

        #region Import : Edit Datagrid
        private void ImportDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DbGlobal.SaveChanges();
            }
            catch (Exception)
            {
                throw;

            }
        }

        private void PArtErsetzenButoon_Click(object sender, RoutedEventArgs e)
        {
            Import.ReplacePArt(AktuellePArtTextBox.Text, NeuerPArtTextBox.Text, DbGlobal);
            ImportTablesComboBox_DropDownClosed(sender, e);
            AktuellePArtTextBox.Text = null;
            NeuerPArtTextBox.Text = null;
        }

        #endregion
        #endregion

        #region Import: Sketches

        #region Import : Skizzen

        #region alte speicher_Button
        /*  Dies ist das alte Code von speicher_Button 
         *  
         *  private void Skizze_Imp_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Title = "My files Browser";
            ofd.Filter = "All Files |*.JPG;*.pdf*;*.ppt;*.pptx";
            System.Windows.Forms.DialogResult dr = ofd.ShowDialog();
            string path = Environment.CurrentDirectory;
            string projectPath = Directory.GetParent(path).Parent.Parent.Parent.Parent.FullName;
            var mainPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "Skizzen");
            string filesPath = projectPath + "\\files";
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                // Read the files
                foreach (String file in ofd.FileNames)
                {
                    string storePath = "";
                    if (!isDicrectoryExist(Path.GetFileNameWithoutExtension(file)[..4], mainPath))
                    {
                        if (Path.GetExtension(file).ToUpper().Equals(".JPG"))
                        {
                            Directory.CreateDirectory(mainPath + "\\JPG" + "\\" + Path.GetFileNameWithoutExtension(file)[..4]);
                            storePath = Path.Combine(mainPath, "JPG", Path.GetFileNameWithoutExtension(file)[..4]);
                        }
                        else if (Path.GetExtension(file).ToUpper().Equals(".PDF"))
                        {
                            Directory.CreateDirectory(mainPath + "\\PDF" + "\\" + Path.GetFileNameWithoutExtension(file)[..4]);
                            storePath = Path.Combine(mainPath, "PDF", Path.GetFileNameWithoutExtension(file)[..4]);
                        }
                        else
                        {
                            Directory.CreateDirectory(mainPath + "\\PowerPoint" + "\\" + Path.GetFileNameWithoutExtension(file)[..4]);
                            storePath = Path.Combine(mainPath, "PowerPoint", Path.GetFileNameWithoutExtension(file)[..4]);
                        }
                    }
                    System.IO.File.Copy(file, storePath + "\\" + Path.GetFileName(file), true);
                }
            }
        }
        private bool isDicrectoryExist(string pad, string filesDirectoryPath)
        {
            //System.Diagnostics.Debug.WriteLine(filesDirectoryPath);
            string[] directories = Directory.GetDirectories(@filesDirectoryPath);
            for (int i = 0; i < directories.Length; i++)
            {
                if (directories[i].Equals(pad))
                    return true;
            }
            return false;
        }*/
        #endregion

        private void SkizzenImportButton_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialogForSketches.ShowDialog() == true)
            {

                Filenames = openFileDialogForSketches.FileNames.ToList();
                Import.ImportSketchesInDataGrid(Filenames, Skizzennames, dataTableForSkizzen);
                ArbeitsauftragRadioButton.IsEnabled = true;
                HauptdatenbestandRadioButton.IsEnabled = true;
                JPGCheckBox.IsEnabled = true;
                PDFCheckBox.IsEnabled = true;
            }
            SkizzenImportDatagrid.ItemsSource = dataTableForSkizzen.DefaultView;
        }

        #endregion

        #region Import: Save Sketches
        private void SkizzenSpeichernButton_Click(object sender, RoutedEventArgs e)
        {
            ImportJpgDisplay.Source = null;
            ImportPdfDisplay.Navigate("about:blank");
            Import.SaveSketchesInDb(dataTableForSkizzen);
            JPGCheckBox.IsChecked = false;
            PDFCheckBox.IsChecked = false;
            JPGCheckBox.IsEnabled = false;
            PDFCheckBox.IsEnabled = false;
            dataTableForMainDatabaseSketches.Rows.Clear();
            dataTableForMainDatabaseSketches.Columns.Clear();
            HauptdatenbestandRadioButton.IsChecked = false;
            ArbeitsauftragRadioButton.IsChecked = false;
            HauptdatenbestandRadioButton.IsEnabled = false;
            ArbeitsauftragRadioButton.IsEnabled = false;
        }
        #endregion

        #region Import : Convert PPT Sketches In To JPG and PDF
        public static void ConvertPptToPdf(string inputFile)
        {
            var sPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "Importierte Skizzen\\");
            string path = sPath + Path.GetFileNameWithoutExtension(inputFile);
            var pptApp = new PowerPoint.Application();
            var pptDocument = pptApp.Presentations.Open((inputFile),
            MsoTriState.msoCTrue,
            //ReadOnly
            MsoTriState.msoFalse,
            MsoTriState.msoFalse);

            pptDocument.ExportAsFixedFormat(path + ".pdf", PpFixedFormatType.ppFixedFormatTypePDF);
            pptDocument.Close();
            pptApp.Quit();

        }
        public static void ConvertPptToJpg(string inputFile)
        {
            var sPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "Importierte Skizzen");
            string path = sPath + Path.GetFileNameWithoutExtension(inputFile);
            var pptApp = new PowerPoint.Application();
            var pptDocument = pptApp.Presentations.Open((inputFile),
            MsoTriState.msoCTrue,
            //ReadOnly
            MsoTriState.msoTrue,
            MsoTriState.msoFalse);

            string imgName = Path.GetFileNameWithoutExtension(inputFile) + ".jpg";

            foreach (Slide slide in pptDocument.Slides)
            {
                slide.Export(sPath + "\\" + imgName, "jpg", Int32.Parse(pptDocument.SlideMaster.Width.ToString()), Int32.Parse(pptDocument.SlideMaster.Height.ToString()));
            }

            pptDocument.Close();
            pptApp.Quit();

        }
        private void JPGCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Filenames = openFileDialogForSketches.FileNames.ToList();
            foreach (string file in Filenames)
            {
                ConvertPptToJpg(file);

                var rowsToUpdate = dataTableForSkizzen.AsEnumerable().Where(r => r.Field<string>("PAD") == Path.GetFileNameWithoutExtension(file));
                foreach (var row in rowsToUpdate)
                {
                    row.SetField("JPG", true);
                }
                dataTableForSkizzen.AcceptChanges();
            }
        }
        private void PDFCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Filenames = openFileDialogForSketches.FileNames.ToList();
            foreach (string file in Filenames)
            {
                ConvertPptToPdf(file);

                var rowsToUpdate = dataTableForSkizzen.AsEnumerable().Where(r => r.Field<string>("PAD") == Path.GetFileNameWithoutExtension(file));
                foreach (var row in rowsToUpdate)
                {
                    row.SetField("PDF", true);
                }
                dataTableForSkizzen.AcceptChanges();
            }
        }

        #endregion

        #region Import : Show Sketches in Datagrid
        private void SkizzenImportDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SkizzenImportDatagrid.SelectedItem != null)
            {
                FindJpgAndPdfSketchesToDisplay();
            }
            else
            {
                //
            }
        }
        private void FindJpgAndPdfSketchesToDisplay()
        {
            var sPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\..\\", "temp", "Importierte Skizzen\\");
            List<string> allFilesInTheDirectory = Directory.GetFiles(sPath, "*.*", SearchOption.AllDirectories).ToList();
            string pad = (SkizzenImportDatagrid.SelectedItem as DataRowView).Row["PAD"].ToString();
            string pathOfJpgSketch = allFilesInTheDirectory.Find(x => x.Contains(pad) && x.Contains("jpg"));
            string pathOfPdfSketch = allFilesInTheDirectory.Find(x => x.Contains(pad) && x.Contains("pdf"));
            pptFile = allFilesInTheDirectory.Find(x => x.Contains(pad) && x.Contains("ppt"));

            if (pathOfJpgSketch != null)
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(pathOfJpgSketch);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                ImportJpgDisplay.Source = bitmapImage;
            }
            else if (pathOfJpgSketch == null && pathOfPdfSketch != null)
            {
                ImportPdfDisplay.Visibility = Visibility.Visible;
                ImportPdfDisplay.Navigate(pathOfPdfSketch);
            }
            else if (pathOfJpgSketch == null && pathOfPdfSketch == null && pptFile != null)
            {
                /*JPGCheckBox_Checked(sender, e);
                FindJPGAndPDFSketchesToDisplay();*/
            }

        }

        #endregion

        #region Import: Check Sketches which are already in Database
        private void HauptdatenbestandPrüfenButton_Click(object sender, RoutedEventArgs e)
        {
            HauptdatenbestandRadioButton_Checked(sender, e);
        }

        private void HauptdatenbestandRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Import.CheckTheSketchesWhichAreAlreadyInDataBase(Filenames, dataTableForMainDatabaseSketches);
            SkizzenImportDatagrid.ItemsSource = dataTableForMainDatabaseSketches.DefaultView;
        }
        private void ArbeitsauftragRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            dataTableForSkizzen.Rows.Clear();
            Import.NewlyImportedSketchesInDG(Filenames, Skizzennames, dataTableForSkizzen);
            SkizzenImportDatagrid.ItemsSource = dataTableForSkizzen.DefaultView;
        }


        #endregion

        #endregion

        #region Export

        

        #region Export : Load to Datagrid
        private void ProtoTablesComboBox_DropDownClosed(object sender, EventArgs e)
        {
            switch (ProtoTablesComboBox.SelectedIndex)
            {
                case 0:
                    LoadExportPp(ProtoPpTable);
                    ItemsCount(ProtoPpTable);
                    break;
                case 1:
                    LoadExportPH(ProtoPhTable);
                    ItemsCount(ProtoPhTable);
                    break;
                case 2:
                    LoadExportPK(ProtoPkTable);
                    ItemsCount(ProtoPkTable);
                    break;
                case 3:
                    LoadExportPL(ProtoPlTable);
                    ItemsCount(ProtoPlTable);
                    break;
                case 4:
                    LoadExportPS(ProtoPsTable);
                    ItemsCount(ProtoPsTable);
                    break;
            }

        }

        private void ExportDatagrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "PAD")
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void LoadExport()
        {
            ProtoPpTable.ItemsSource = DbGlobal.Pp.ToList();
            ProtoPhTable.ItemsSource = DbGlobal.Ph.ToList();
            ProtoPkTable.ItemsSource = DbGlobal.Pk.ToList();
            ProtoPlTable.ItemsSource = DbGlobal.Pl.ToList();
            ProtoPsTable.ItemsSource = DbGlobal.Ps.ToList();
        }

        private void LoadExportPp(ItemsControl dataGrid)
        {
            //dataGrid.DataContext = null;
            //dataGrid.ItemsSource = DbGlobal.Pp.ToList();
            ProtoFilterGridPP.IsEnabled = true; ProtoFilterGridPH.IsEnabled = false; ProtoFilterGridPK.IsEnabled = false; ProtoFilterGridPL.IsEnabled = false; ProtoFilterGridPS.IsEnabled = false;
            ProtoFilterGridPP.Visibility = Visibility.Visible; ProtoFilterGridPH.Visibility = Visibility.Hidden; ProtoFilterGridPK.Visibility = Visibility.Hidden; ProtoFilterGridPL.Visibility = Visibility.Hidden; ProtoFilterGridPS.Visibility = Visibility.Hidden;
            ProtoPpTable.Visibility = Visibility.Visible; ProtoPhTable.Visibility = Visibility.Hidden; ProtoPkTable.Visibility = Visibility.Hidden; ProtoPlTable.Visibility = Visibility.Hidden; ProtoPsTable.Visibility = Visibility.Hidden;
        }
        private void LoadExportPH(ItemsControl dataGrid)
        {
            ProtoFilterGridPP.IsEnabled = false; ProtoFilterGridPH.IsEnabled = true; ProtoFilterGridPK.IsEnabled = false; ProtoFilterGridPL.IsEnabled = false; ProtoFilterGridPS.IsEnabled = false;
            ProtoFilterGridPP.Visibility = Visibility.Hidden; ProtoFilterGridPH.Visibility = Visibility.Visible; ProtoFilterGridPK.Visibility = Visibility.Hidden; ProtoFilterGridPL.Visibility = Visibility.Hidden; ProtoFilterGridPS.Visibility = Visibility.Hidden;
            ProtoPpTable.Visibility = Visibility.Hidden; ProtoPhTable.Visibility = Visibility.Visible; ProtoPkTable.Visibility = Visibility.Hidden; ProtoPlTable.Visibility = Visibility.Hidden; ProtoPsTable.Visibility = Visibility.Hidden;
        }
        private void LoadExportPK(ItemsControl dataGrid)
        {
            ProtoFilterGridPP.IsEnabled = true; ProtoFilterGridPH.IsEnabled = false; ProtoFilterGridPK.IsEnabled = true; ProtoFilterGridPL.IsEnabled = false; ProtoFilterGridPS.IsEnabled = false;
            ProtoFilterGridPP.Visibility = Visibility.Hidden; ProtoFilterGridPH.Visibility = Visibility.Hidden; ProtoFilterGridPK.Visibility = Visibility.Visible; ProtoFilterGridPL.Visibility = Visibility.Hidden; ProtoFilterGridPS.Visibility = Visibility.Hidden;
            ProtoPpTable.Visibility = Visibility.Hidden; ProtoPhTable.Visibility = Visibility.Hidden; ProtoPkTable.Visibility = Visibility.Visible; ProtoPlTable.Visibility = Visibility.Hidden; ProtoPsTable.Visibility = Visibility.Hidden;
        }

        private void LoadExportPL(ItemsControl dataGrid)
        {
            ProtoFilterGridPP.IsEnabled = true; ProtoFilterGridPH.IsEnabled = false; ProtoFilterGridPK.IsEnabled = false; ProtoFilterGridPL.IsEnabled = true; ProtoFilterGridPS.IsEnabled = false;
            ProtoFilterGridPP.Visibility = Visibility.Hidden; ProtoFilterGridPH.Visibility = Visibility.Hidden; ProtoFilterGridPK.Visibility = Visibility.Hidden; ProtoFilterGridPL.Visibility = Visibility.Visible; ProtoFilterGridPS.Visibility = Visibility.Hidden;
            ProtoPpTable.Visibility = Visibility.Hidden; ProtoPhTable.Visibility = Visibility.Hidden; ProtoPkTable.Visibility = Visibility.Hidden; ProtoPlTable.Visibility = Visibility.Visible; ProtoPsTable.Visibility = Visibility.Hidden;
        }
        private void LoadExportPS(ItemsControl dataGrid)
        {
            ProtoFilterGridPP.IsEnabled = true; ProtoFilterGridPH.IsEnabled = false; ProtoFilterGridPK.IsEnabled = false; ProtoFilterGridPL.IsEnabled = false; ProtoFilterGridPS.IsEnabled = true;
            ProtoFilterGridPP.Visibility = Visibility.Hidden; ProtoFilterGridPH.Visibility = Visibility.Hidden; ProtoFilterGridPK.Visibility = Visibility.Hidden; ProtoFilterGridPL.Visibility = Visibility.Hidden; ProtoFilterGridPS.Visibility = Visibility.Visible;
            ProtoPpTable.Visibility = Visibility.Hidden; ProtoPhTable.Visibility = Visibility.Hidden; ProtoPkTable.Visibility = Visibility.Hidden; ProtoPlTable.Visibility = Visibility.Hidden; ProtoPsTable.Visibility = Visibility.Visible;
        }
        #endregion

        #region Export : Filter        

        private void FilterExport()
        {
            ProtoPpTable.ItemsSource = null;
            ProtoPhTable.ItemsSource = null;
            ProtoPkTable.ItemsSource = null;
            ProtoPlTable.ItemsSource = null;
            ProtoPsTable.ItemsSource = null;
            var filteredPp = DbGlobal.Pp.OrderBy(p => p.PAD).ToList();
            var filteredPh = DbGlobal.Ph.OrderBy(p => p.PAD).ToList();
            var filteredPk = DbGlobal.Pk.OrderBy(p => p.PAD).ToList();
            var filteredPl = DbGlobal.Pl.OrderBy(p => p.PAD).ToList();
            var filteredPs = DbGlobal.Ps.OrderBy(p => p.PAD).ToList();

            // new database to filter

            var filteredAv = DbFilter.Avani.ToList();
            var filteredIVB = DbFilter.GIvlBasis.ToList();
            var filteredIVK = DbFilter.GIvlKoordinaten.ToList();

            if (!string.IsNullOrEmpty(Strecke.Text))
            {

                filteredIVB.RemoveAll(a => !a.Strecke.Contains(Strecke.Text));
                filteredPp.RemoveAll(x => !filteredIVB.Exists(y => y.Planadresse.ToUpper() == x.Blattschnitt));

                if (Fangradius1Checkbox.IsChecked == false && Fangradius2Checkbox.IsChecked == false)
                {
                    filteredPk.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                    filteredPh.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                    filteredPl.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                    filteredPs.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                }
            }


            if (!string.IsNullOrEmpty(KMvonPP.Text))
            {
                double textinputfrom = Import.CalculateKmInDatabankFormat(Convert.ToDouble(KMvonPP.Text), 0d);

                filteredIVB.RemoveAll(a => Convert.ToDouble(a.KmdbAnfangCh13) <= textinputfrom);

                filteredPp.RemoveAll(x => !filteredIVB.Exists(y => y.Planadresse.ToUpper() == x.Blattschnitt));
                if (Fangradius1Checkbox.IsChecked == false && Fangradius2Checkbox.IsChecked == false)
                {
                    filteredPk.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                    filteredPh.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                    filteredPl.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                    filteredPs.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                }
            }

            if (!string.IsNullOrEmpty(KMbisPP.Text))
            {
                double textinputto = Import.CalculateKmInDatabankFormat(Convert.ToDouble(KMbisPP.Text), 0d);
                filteredIVB.RemoveAll(a => Convert.ToDouble(a.KmdbAnfangCh13) >= textinputto);

                filteredPp.RemoveAll(x => !filteredIVB.Exists(y => y.Planadresse.ToUpper() == x.Blattschnitt));

                if (Fangradius1Checkbox.IsChecked == false && Fangradius2Checkbox.IsChecked == false)
                {
                    filteredPk.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                    filteredPh.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                    filteredPl.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                    filteredPs.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                }

            }

            //pp table fangradius filter
            if (Fangradius1Checkbox.IsChecked == true && Fangradius2Checkbox.IsChecked==false )
            {
                    filteredAv.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.Pad));
                    Fangradius.FangradiusPS0PS1(filteredAv, Convert.ToDouble(FangradiusPS0toPs1.Text), filteredPp);

                filteredPk.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPh.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
               
            }

            if (Fangradius2Checkbox.IsChecked == true && Fangradius1Checkbox.IsChecked == false)
            { 
                filteredAv.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.Pad));
                Fangradius.FangradiusPS2PS4(filteredAv, Convert.ToDouble(FangradiusPS2toPS4.Text), filteredPp);

                filteredPk.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPh.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
            }

            if (Fangradius1Checkbox.IsChecked == true && Fangradius2Checkbox.IsChecked == true)
            {
                filteredAv.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.Pad));
                Fangradius.FangradiusPS0PS4(filteredAv, Convert.ToDouble(FangradiusPS0toPs1.Text), Convert.ToDouble(FangradiusPS2toPS4.Text), filteredPp);

                filteredPk.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPh.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));

            }

            //pp table checkbox filter
            if (PS0check.IsChecked == false)
            {
                filteredPp.RemoveAll(a => a.PArt.Contains("PS0"));

                filteredPh.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPk.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
            }
            if (PS1check.IsChecked == false)
            {
                filteredPp.RemoveAll(a => a.PArt.Contains("PS1"));

                filteredPh.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPk.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
            }
            if (PS2check.IsChecked == false)
            {
                filteredPp.RemoveAll(a => a.PArt.Contains("PS2"));

                filteredPh.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPk.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
            }
            if (PS3check.IsChecked == false)
            {
                filteredPp.RemoveAll(a => a.PArt.Contains("PS3"));

                filteredPh.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPk.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
            }
            if (PS4check.IsChecked == false)
            {
                filteredPp.RemoveAll(a => a.PArt.Contains("PS4"));

                filteredPh.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPk.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPp.Exists(y => y.PAD == x.PAD));
            }

            //Pk filter
            if (!string.IsNullOrEmpty(XFilter.Text))
            {
                filteredPk.RemoveAll(a => !a.X.Contains(XFilter.Text));
                filteredPp.RemoveAll(x => !filteredPk.Exists(y => y.PAD == x.PAD));
                filteredPh.RemoveAll(x => !filteredPk.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPk.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPk.Exists(y => y.PAD == x.PAD));
            }
            if (!string.IsNullOrEmpty(YFilter.Text))
            {
                filteredPk.RemoveAll(a => !a.Y.Contains(YFilter.Text));
                filteredPp.RemoveAll(x => !filteredPk.Exists(y => y.PAD == x.PAD));
                filteredPh.RemoveAll(x => !filteredPk.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPk.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPk.Exists(y => y.PAD == x.PAD));
            }
            if (!string.IsNullOrEmpty(ZFilter.Text))
            {
                filteredPk.RemoveAll(a => !a.Z.Contains(ZFilter.Text));
                filteredPp.RemoveAll(x => !filteredPk.Exists(y => y.PAD == x.PAD));
                filteredPh.RemoveAll(x => !filteredPk.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPk.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPk.Exists(y => y.PAD == x.PAD));
            }
            //Pl Filter
           
            if (ER0Checkbox.IsChecked == false)
            {
                filteredPl.RemoveAll(a => a.LSys.Contains("ER0"));
                filteredPp.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
                filteredPh.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
                filteredPk.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
            }
            if (FR0Checkbox.IsChecked == false)
            {
                filteredPl.RemoveAll(a => a.LSys.Contains("FR0"));
                filteredPp.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
                filteredPh.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
                filteredPk.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
            }
            
            if (DR0Checkbox.IsChecked == false)
            {
                filteredPl.RemoveAll(a => a.LSys.Contains("DR0"));
                filteredPp.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
                filteredPh.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
                filteredPk.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
            }
            
            if (!string.IsNullOrEmpty(PLTextboxFilter.Text))
            {
                var inputText = PLTextboxFilter.Text;
                filteredPl.RemoveAll(a => !a.LSys.Contains(inputText));
                filteredPp.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
                filteredPh.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
                filteredPk.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPl.Exists(y => y.PAD == x.PAD));
            }
            
            //Ph filter
            if (O00Checkbox.IsChecked == false)
            {
                filteredPh.RemoveAll(a => a.HSys.Contains("O00"));
                filteredPp.RemoveAll(x => !filteredPh.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPh.Exists(y => y.PAD == x.PAD));
                filteredPk.RemoveAll(x => !filteredPh.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPh.Exists(y => y.PAD == x.PAD));
            }
            if (R00Checkbox.IsChecked == false)
            {
                filteredPh.RemoveAll(a => a.HSys.Contains("R00"));
                filteredPp.RemoveAll(x => !filteredPh.Exists(y => y.PAD == x.PAD));
                filteredPl.RemoveAll(x => !filteredPh.Exists(y => y.PAD == x.PAD));
                filteredPk.RemoveAll(x => !filteredPh.Exists(y => y.PAD == x.PAD));
                filteredPs.RemoveAll(x => !filteredPh.Exists(y => y.PAD == x.PAD));
            }
           
                if (!string.IsNullOrEmpty(PHCustomTextbox.Text))
                {
                    var inputText = PHCustomTextbox.Text;
                    filteredPh.RemoveAll(a => !a.HSys.Contains(inputText));
                    filteredPp.RemoveAll(x => !filteredPh.Exists(y => y.PAD == x.PAD));
                    filteredPl.RemoveAll(x => !filteredPh.Exists(y => y.PAD == x.PAD));
                    filteredPk.RemoveAll(x => !filteredPh.Exists(y => y.PAD == x.PAD));
                    filteredPs.RemoveAll(x => !filteredPh.Exists(y => y.PAD == x.PAD));
                }
            

            ProtoPpTable.ItemsSource = filteredPp;
            ProtoPhTable.ItemsSource = filteredPh;
            ProtoPkTable.ItemsSource = filteredPk;
            ProtoPlTable.ItemsSource = filteredPl;
            ProtoPsTable.ItemsSource = filteredPs;
        }

        #endregion

        #region Export : Methode

        string skizzePath;

        private void skizzenExportButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = true;
            System.Windows.Forms.DialogResult dialogResult = folderBrowserDialog.ShowDialog();

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                skizzenExportBox.Text = folderBrowserDialog.SelectedPath;
                skizzePath = folderBrowserDialog.SelectedPath;
            }
        }

        private void ProtoFinalButton_Click(object sender, RoutedEventArgs e)
        {
            if (inputExportBox.Text == "")
            {
                MessageBox.Show("Bitte geben Sie den Dateinamen.");
            }
            else
            {
                if (xlsCheckBox.IsChecked == true || xlsAutoCheckBox.IsChecked == true || xlsEinfachCheckBox.IsChecked == true ||
                dbbCheckBox.IsChecked == true || napCheckBox.IsChecked == true || csvCheckBox.IsChecked == true ||
                pdfExportCheckBox.IsChecked == true || jpgExportCheckBox.IsChecked == true || pptExportCheckBox.IsChecked == true)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.FileName = inputExportBox.Text;

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string path = Path.GetDirectoryName(saveFileDialog.FileName);

                        //Excel
                        if (xlsCheckBox.IsChecked == true)
                        {
                            string PathExcel = $@"{path}\{inputExportBox.Text}.xlsx";

                            var dPp = ProtoPpTable.ItemsSource.Cast<Pp>();
                            System.Data.DataTable excelPp = Export.ToDataTable(dPp.ToList());
                            var dPh = ProtoPhTable.ItemsSource.Cast<Ph>();
                            System.Data.DataTable excelPh = Export.ToDataTable(dPh.ToList());
                            var dPk = ProtoPkTable.ItemsSource.Cast<Pk>();
                            System.Data.DataTable excelPk = Export.ToDataTable(dPk.ToList());
                            var dPl = ProtoPlTable.ItemsSource.Cast<Pl>();
                            System.Data.DataTable excelPl = Export.ToDataTable(dPl.ToList());
                            var dPs = ProtoPsTable.ItemsSource.Cast<Ps>();
                            System.Data.DataTable excelPs = Export.ToDataTable(dPs.ToList());
                            if (PPCheckBox.IsChecked == false) excelPp = null;
                            if (PHCheckBox.IsChecked == false) excelPh = null;
                            if (PKCheckBox.IsChecked == false) excelPk = null;
                            if (PLCheckBox.IsChecked == false) excelPl = null;
                            if (PSCheckBox.IsChecked == false) excelPs = null;
                            Export.ToExcelFile(excelPp, excelPh, excelPk, excelPl, excelPs, PathExcel);
                        }

                        //Auto Excel
                        if (xlsAutoCheckBox.IsChecked == true)
                        {
                            string PathExcel = $@"{path}\{inputExportBox.Text}AutoDat.xlsx";

                            List<Pp> autoPp = ProtoPpTable.ItemsSource.Cast<Pp>().ToList();
                            List<Ph> autoPh = ProtoPhTable.ItemsSource.Cast<Ph>().ToList();
                            List<Pk> autoPk = ProtoPkTable.ItemsSource.Cast<Pk>().ToList();
                            List<Pl> autoPl = ProtoPlTable.ItemsSource.Cast<Pl>().ToList();
                            List<Ps> autoPs = ProtoPsTable.ItemsSource.Cast<Ps>().ToList();
                            if (PHCheckBox.IsChecked == false) autoPh = null;
                            if (PKCheckBox.IsChecked == false) autoPk = null;
                            if (PLCheckBox.IsChecked == false) autoPl = null;
                            if (PSCheckBox.IsChecked == false) autoPs = null;
                            Export.ToExcelFileAuto(autoPp, autoPh, autoPk, autoPl, autoPs, PathExcel);
                        }

                        //Einfach Excel
                        if (xlsEinfachCheckBox.IsChecked == true)
                        {
                            string PathExcel = $@"{path}\{inputExportBox.Text}Einfach.xlsx";

                            List<Pp> einfachPp = ProtoPpTable.ItemsSource.Cast<Pp>().ToList();
                            List<Ph> einfachPh = ProtoPhTable.ItemsSource.Cast<Ph>().ToList();
                            List<Pk> einfachPk = ProtoPkTable.ItemsSource.Cast<Pk>().ToList();
                            List<Pl> einfachPl = ProtoPlTable.ItemsSource.Cast<Pl>().ToList();
                            List<Ps> einfachPs = ProtoPsTable.ItemsSource.Cast<Ps>().ToList();
                            if (PHCheckBox.IsChecked == false) einfachPh = null;
                            if (PKCheckBox.IsChecked == false) einfachPk = null;
                            if (PLCheckBox.IsChecked == false) einfachPl = null;
                            if (PSCheckBox.IsChecked == false) einfachPs = null;
                            Export.ToExcelFileEinfach(einfachPp, einfachPh, einfachPk, einfachPl, einfachPs, PathExcel);
                        }

                        //dbb Format
                        if (dbbCheckBox.IsChecked == true)
                        {
                            string PathDbb = $@"{path}\{inputExportBox.Text}.dbb";
                            List<Pp> dbbPp = ProtoPpTable.ItemsSource.Cast<Pp>().ToList();
                            List<Ph> dbbPh = ProtoPhTable.ItemsSource.Cast<Ph>().ToList();
                            List<Pl> dbbPl = ProtoPlTable.ItemsSource.Cast<Pl>().ToList();
                            List<Ps> dbbPs = ProtoPsTable.ItemsSource.Cast<Ps>().ToList();

                            Export.ExportDbb(dbbPp, dbbPh, dbbPl, dbbPs, PathDbb);
                        }

                        //nap Format
                        if (napCheckBox.IsChecked == true)
                        {
                            string PathNap = $@"{path}\{inputExportBox.Text}.nap";

                            List<Pp> napPp = ProtoPpTable.ItemsSource.Cast<Pp>().ToList();
                            List<Ph> napPh = ProtoPhTable.ItemsSource.Cast<Ph>().ToList();
                            List<Pk> napPk = ProtoPkTable.ItemsSource.Cast<Pk>().ToList();
                            List<Pl> napPl = ProtoPlTable.ItemsSource.Cast<Pl>().ToList();
                            List<Ps> napPs = ProtoPsTable.ItemsSource.Cast<Ps>().ToList();
                            if (PHCheckBox.IsChecked == false) napPh = null;
                            if (PKCheckBox.IsChecked == false) napPk = null;
                            if (PLCheckBox.IsChecked == false) napPl = null;
                            if (PSCheckBox.IsChecked == false) napPs = null;
                            Export.ExportNap(napPp, napPh, napPk, napPl, napPs, PathNap);
                        }
                        //csv Format
                        if (csvCheckBox.IsChecked == true)
                        {
                            string PathCsv = $@"{path}\{inputExportBox.Text}.csv";

                            List<Pp> csvPp = ProtoPpTable.ItemsSource.Cast<Pp>().ToList();
                            List<Ph> csvPh = ProtoPhTable.ItemsSource.Cast<Ph>().ToList();
                            List<Pk> csvPk = ProtoPkTable.ItemsSource.Cast<Pk>().ToList();
                            List<Pl> csvPl = ProtoPlTable.ItemsSource.Cast<Pl>().ToList();
                            List<Ps> csvPs = ProtoPsTable.ItemsSource.Cast<Ps>().ToList();
                            if (PHCheckBox.IsChecked == false) csvPh = null;
                            if (PKCheckBox.IsChecked == false) csvPk = null;
                            if (PLCheckBox.IsChecked == false) csvPl = null;
                            if (PSCheckBox.IsChecked == false) csvPs = null;
                            Export.ToCsvFile(csvPp, csvPh, csvPk, csvPl, csvPs, PathCsv);
                        }

                        //Skizze Export
                        if (skizzenExportBox.Text == "")
                        {
                            MessageBox.Show("Bitte geben Sie den Pfad der Skizzen.");
                        }
                        else
                        {
                            if (pdfExportCheckBox.IsChecked == true)
                            {
                                string filePath = $@"{path}\{inputExportBox.Text}";

                                List<Pp> Pp = ProtoPpTable.ItemsSource.Cast<Pp>().ToList();
                                Export.SkizzeExportPDF(Pp, filePath, skizzePath);
                            }
                            if (jpgExportCheckBox.IsChecked == true)
                            {
                                string filePath = $@"{path}\{inputExportBox.Text}";

                                List<Pp> Pp = ProtoPpTable.ItemsSource.Cast<Pp>().ToList();
                                Export.SkizzeExportJPG(Pp, filePath, skizzePath);
                            }
                            if (pptExportCheckBox.IsChecked == true)
                            {
                                string filePath = $@"{path}\{inputExportBox.Text}";

                                List<Pp> Pp = ProtoPpTable.ItemsSource.Cast<Pp>().ToList();
                                Export.SkizzeExportPPT(Pp, filePath, skizzePath);
                            }
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Sie müssen mindesten 1 Format oder Skizzen wählen.");
                }
            }
            

        }


        #endregion

        #region Export : Event

        #region Checkbox Event
        private void xlsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            GridTableCheckBox.Visibility = Visibility.Visible;
        }

        private void xlsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (dbbCheckBox.IsChecked == true || csvCheckBox.IsChecked == true || napCheckBox.IsChecked == true || 
                xlsEinfachCheckBox.IsChecked == true || xlsAutoCheckBox.IsChecked == true)
            {
                GridTableCheckBox.Visibility = Visibility.Visible;
            }
            else
            {
                GridTableCheckBox.Visibility = Visibility.Hidden;
            }
        }

        private void dbbCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            GridTableCheckBox.Visibility = Visibility.Visible;
            PPCheckBox.IsChecked = true;
            PHCheckBox.IsChecked = true;
            PLCheckBox.IsChecked = true;
            PSCheckBox.IsChecked = true;
        }

        private void dbbCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (xlsCheckBox.IsChecked == true || csvCheckBox.IsChecked == true || napCheckBox.IsChecked == true ||
                 xlsEinfachCheckBox.IsChecked == true || xlsAutoCheckBox.IsChecked == true)
            {
                GridTableCheckBox.Visibility = Visibility.Visible;
                PHCheckBox.IsChecked = false;
                PLCheckBox.IsChecked = false;
                PSCheckBox.IsChecked = false;
            }
            else
            {
                GridTableCheckBox.Visibility = Visibility.Hidden;
                PPCheckBox.IsChecked = false;
                PHCheckBox.IsChecked = false;
                PLCheckBox.IsChecked = false;
                PSCheckBox.IsChecked = false;
            }
        }

        private void csvCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            GridTableCheckBox.Visibility = Visibility.Visible;
            PPCheckBox.IsChecked = true;
        }

        private void csvCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (dbbCheckBox.IsChecked == true || xlsCheckBox.IsChecked == true || napCheckBox.IsChecked == true ||
                xlsEinfachCheckBox.IsChecked == true || xlsAutoCheckBox.IsChecked == true)
            {
                GridTableCheckBox.Visibility = Visibility.Visible;
            }
            else
            {
                GridTableCheckBox.Visibility = Visibility.Hidden;
            }
        }

        private void napCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            GridTableCheckBox.Visibility = Visibility.Visible;
            PPCheckBox.IsChecked = true;
        }

        private void napCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (dbbCheckBox.IsChecked == true || csvCheckBox.IsChecked == true || xlsCheckBox.IsChecked == true ||
                xlsEinfachCheckBox.IsChecked == true || xlsAutoCheckBox.IsChecked == true)
            {
                GridTableCheckBox.Visibility = Visibility.Visible;
            }
            else
            {
                GridTableCheckBox.Visibility = Visibility.Hidden;
            }
        }

        private void xlsEinfachCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            GridTableCheckBox.Visibility = Visibility.Visible;
            PPCheckBox.IsChecked = true;
        }

        private void xlsEinfachCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (dbbCheckBox.IsChecked == true || csvCheckBox.IsChecked == true || napCheckBox.IsChecked == true ||
                xlsCheckBox.IsChecked == true || xlsAutoCheckBox.IsChecked == true)
            {
                GridTableCheckBox.Visibility = Visibility.Visible;
            }
            else
            {
                GridTableCheckBox.Visibility = Visibility.Hidden;
            }
        }

        private void xlsAutoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            GridTableCheckBox.Visibility = Visibility.Visible;
            PPCheckBox.IsChecked = true;
        }

        private void xlsAutoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (dbbCheckBox.IsChecked == true || csvCheckBox.IsChecked == true || napCheckBox.IsChecked == true ||
                xlsEinfachCheckBox.IsChecked == true || xlsCheckBox.IsChecked == true)
            {
                GridTableCheckBox.Visibility = Visibility.Visible;
            }
            else
            {
                GridTableCheckBox.Visibility = Visibility.Hidden;
            }
        }

        #endregion
        private void ExportDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged()
        {

        }

        private void ProtoTable_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Cancel = CancelGenerationOfUnwantedColumns(e.Column.Header.ToString());

            if (ProtoPpTable.IsVisible == true)
                ItemsCount(ProtoPpTable);
            if (ProtoPsTable.IsVisible == true)
                ItemsCount(ProtoPsTable);
            if (ProtoPkTable.IsVisible == true)
                ItemsCount(ProtoPkTable);
            if (ProtoPhTable.IsVisible == true)
                ItemsCount(ProtoPhTable);
            if (ProtoPlTable.IsVisible == true)
                ItemsCount(ProtoPlTable);
        }

        private void ItemsCount(DataGrid datagrid)
        {
            ProtoEntryCountTextBlock.Text = datagrid.Items.Count.ToString();
        }

        private void PADFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
           if (_skipEvent) return;
           
        }

        private void PArtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
          
        }

        private void BlattschnittFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
            
        }

        private void XFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
            
        }

        private void YFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
            
        }

        private void ZFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;

        }

        private void ER0Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (_skipEvent) return;
            
        }

        private void FR0Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (_skipEvent) return;
            
        }

        private void DR0Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (_skipEvent) return;
            
        }
        
        private void PKTextboxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
        }

        private void ER0Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
           
        }

        private void FR0Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            
        }

        private void DR0Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
        }

        private void R00Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (_skipEvent) return;
            
        }

        private void R00Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_skipEvent) return;
           
           
        }

        private void O00Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (_skipEvent) return;
            
        }

        private void O00Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_skipEvent) return;
            
            
        }

        private void PHCustomTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PHCustomCheckbox_Checked(object sender, RoutedEventArgs e)
        {
           
        }

        private void PPCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void importDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void FangradiusPS0toPs1_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
        }

        private void FangradiusPS2toPS4_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
        }
        private void PS0check_Checked(object sender, RoutedEventArgs e)
        {
            if (_skipEvent) return;
        }
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterExport();
        }




        #endregion

        #endregion

        private void PLTextboxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_skipEvent) return;
        }
    }
}

