using FestpunktDB.Business.Entities;
using FestpunktDB.Business.EntitiesImport;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using FestpunktDB.Business.ImportService;
using FestpunktDB.Business.DataServices;


namespace FestpunktDB.GUI
{
    public partial class Conflict : Form
    {


        List<KeyValuePair<Pp, ImportPp>> conflictsPp = new List<KeyValuePair<Pp, ImportPp>>();
        List<KeyValuePair<Ph, ImportPh>> conflictsPh = new List<KeyValuePair<Ph, ImportPh>>();
        List<KeyValuePair<Pk, ImportPk>> conflictsPk = new List<KeyValuePair<Pk, ImportPk>>();
        List<KeyValuePair<Pl, ImportPl>> conflictsPl = new List<KeyValuePair<Pl, ImportPl>>();
        List<KeyValuePair<Ps, ImportPs>> conflictsPs = new List<KeyValuePair<Ps, ImportPs>>();

        // conflicts PAD lists
        public static List<string> removedOriginalDataPp = new List<string>();
        public static List<string> removednewDataPp = new List<string>();

        public static List<string> removedOriginalDataPh = new List<string>();
        public static List<string> removednewDataPh = new List<string>();

        public static List<string> removedOriginalDataPk = new List<string>();
        public static List<string> removednewDataPk = new List<string>();

        public static List<string> removedOriginalDataPl = new List<string>();
        public static List<string> removednewDataPl = new List<string>();

        public static List<string> removedOriginalDataPs = new List<string>();
        public static List<string> removednewDataPs = new List<string>();

        int mergeCount = 1;

        public Conflict()
        {
            InitializeComponent();

        }

        public Conflict(List<KeyValuePair<Pp, ImportPp>> conflictsPp,
            List<KeyValuePair<Ph, ImportPh>> conflictsPh,
            List<KeyValuePair<Pk, ImportPk>> conflictsPk,
            List<KeyValuePair<Pl, ImportPl>> conflictsPl,
            List<KeyValuePair<Ps, ImportPs>> conflictsPs)
        {
            this.conflictsPp = conflictsPp;
            this.conflictsPh = conflictsPh;
            this.conflictsPk = conflictsPk;
            this.conflictsPl = conflictsPl;
            this.conflictsPs = conflictsPs;

            /*System.Diagnostics.Debug.WriteLine(this.conflictsPp.Count);
            System.Diagnostics.Debug.WriteLine(this.conflictsPh.Count);
            System.Diagnostics.Debug.WriteLine(this.conflictsPk.Count);
            System.Diagnostics.Debug.WriteLine(this.conflictsPl.Count);
            System.Diagnostics.Debug.WriteLine(this.conflictsPs.Count);
            */

            InitializeComponent();
            if (conflictsPp.Count > 0)
            {
                FillTablesPp();
                titleLabel.Text = "Konflikt Pp";
                mergeCount = 1;
            }
            else if (conflictsPh.Count > 0)
            {
                FillTablesPh();
                titleLabel.Text = "Konflikt Ph";
                mergeCount = 2;
            }
            else if (conflictsPk.Count > 0)
            {
                FillTablesPk();
                titleLabel.Text = "Konflikt Pk";
                mergeCount = 3;
            }
            else if (conflictsPl.Count > 0)
            {
                FillTablesPl();
                titleLabel.Text = "Konflikt Pl";
                mergeCount = 4;
            }
            else if (conflictsPs.Count > 0)
            {
                FillTablesPs();
                titleLabel.Text = "Konflikt Ps";
                mergeCount = 5;
            }
        }

        private void FillTablesPh()
        {
            DataTable dtEmp1 = new DataTable();
            DataTable dtEmp2 = new DataTable();
            // add column to datatable  
            int count = typeof(Ph).GetProperties().Length;
            int count2 = typeof(ImportPh).GetProperties().Length;
            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "Chk";
            dgvCmb.FalseValue = false;
            dgvCmb.IndeterminateValue = false;
            dgvCmb.HeaderText = "CheckBox";
            dataGridView1.Columns.Add(dgvCmb);

            DataGridViewCheckBoxColumn dgvCmb2 = new DataGridViewCheckBoxColumn();
            dgvCmb2.ValueType = typeof(bool);
            dgvCmb2.Name = "Chk";
            dgvCmb2.FalseValue = false;
            dgvCmb2.HeaderText = "CheckBox";
            dataGridView2.Columns.Add(dgvCmb2);

            for (int i = 0; i < count; i++)
            {
                dtEmp1.Columns.Add("data" + i, typeof(string));
            }
            for (int i = 0; i < count2; i++)
            {
                dtEmp2.Columns.Add("dataaa" + i, typeof(string));
            }
            conflictsPh.ForEach(delegate (KeyValuePair<Ph, ImportPh> obj)
            {
                Type t = obj.Key.GetType();
                PropertyInfo[] properties = t.GetProperties();
                string[] fields = new string[properties.Length];
                int i = 0;
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetValue(obj.Key, null) != null)
                    {
                        fields[i] = property.GetValue(obj.Key, null).ToString();

                    }
                    else
                        fields[i] = "";
                    i++;
                }
                dtEmp1.Rows.Add(fields);

                Type t2 = obj.Value.GetType();
                PropertyInfo[] properties2 = t2.GetProperties();
                string[] fields2 = new string[properties2.Length];
                i = 0;
                foreach (PropertyInfo property in properties2)
                {
                    if (property.GetValue(obj.Value, null) != null)
                    {
                        fields2[i] = property.GetValue(obj.Value, null).ToString();
                    }
                    else
                    {
                        fields2[i] = "";
                    }
                    i++;
                }
                dtEmp2.Rows.Add(fields2);
            });
            dataGridView1.DataSource = dtEmp1;
            dataGridView2.DataSource = dtEmp2;

            System.Threading.Thread t = new System.Threading.Thread(InitilizeColors);
            t.Start();

        }

        private void FillTablesPp()
        {
            DataTable dtEmp1 = new DataTable();
            DataTable dtEmp2 = new DataTable();
            // add column to datatable  
            int count = typeof(Pp).GetProperties().Length;
            int count2 = typeof(ImportPp).GetProperties().Length;
            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "Chk";
            dgvCmb.FalseValue = false;
            dgvCmb.IndeterminateValue = false;
            dgvCmb.HeaderText = "CheckBox";
            dataGridView1.Columns.Add(dgvCmb);

            DataGridViewCheckBoxColumn dgvCmb2 = new DataGridViewCheckBoxColumn();
            dgvCmb2.ValueType = typeof(bool);
            dgvCmb2.Name = "Chk";
            dgvCmb2.FalseValue = false;
            dgvCmb2.HeaderText = "CheckBox";
            dataGridView2.Columns.Add(dgvCmb2);

            for (int i = 0; i < count; i++)
            {
                dtEmp1.Columns.Add("data" + i, typeof(string));
            }
            for (int i = 0; i < count2; i++)
            {
                dtEmp2.Columns.Add("dataaa" + i, typeof(string));
            }
            conflictsPp.ForEach(delegate (KeyValuePair<Pp, ImportPp> obj)
            {
                Type t = obj.Key.GetType();
                PropertyInfo[] properties = t.GetProperties();
                string[] fields = new string[properties.Length];
                int i = 0;

                foreach (PropertyInfo property in properties)
                {
                    if (property.GetValue(obj.Key, null) != null)
                    {
                        fields[i] = property.GetValue(obj.Key, null).ToString();

                    }
                    else
                        fields[i] = "";
                    i++;
                }
                dtEmp1.Rows.Add(fields);

                Type t2 = obj.Value.GetType();
                PropertyInfo[] properties2 = t2.GetProperties();
                string[] fields2 = new string[properties2.Length];
                i = 0;
                foreach (PropertyInfo property in properties2)
                {
                    if (property.GetValue(obj.Value, null) != null)
                    {
                        fields2[i] = property.GetValue(obj.Value, null).ToString();
                    }
                    else
                    {
                        fields2[i] = "";
                    }
                    i++;
                }
                dtEmp2.Rows.Add(fields2);
            });
            dataGridView1.DataSource = dtEmp1;
            dataGridView2.DataSource = dtEmp2;

            System.Threading.Thread t = new System.Threading.Thread(InitilizeColors);
            t.Start();

        }

        private void FillTablesPk()
        {
            DataTable dtEmp1 = new DataTable();
            DataTable dtEmp2 = new DataTable();
            // add column to datatable  
            int count = typeof(Pk).GetProperties().Length;
            int count2 = typeof(ImportPk).GetProperties().Length;
            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "Chk";
            dgvCmb.FalseValue = false;
            dgvCmb.IndeterminateValue = false;
            dgvCmb.HeaderText = "CheckBox";
            dataGridView1.Columns.Add(dgvCmb);

            DataGridViewCheckBoxColumn dgvCmb2 = new DataGridViewCheckBoxColumn();
            dgvCmb2.ValueType = typeof(bool);
            dgvCmb2.Name = "Chk";
            dgvCmb2.FalseValue = false;
            dgvCmb2.HeaderText = "CheckBox";
            dataGridView2.Columns.Add(dgvCmb2);

            for (int i = 0; i < count; i++)
            {
                dtEmp1.Columns.Add("data" + i, typeof(string));
            }
            for (int i = 0; i < count2; i++)
            {
                dtEmp2.Columns.Add("dataaa" + i, typeof(string));
            }
            conflictsPk.ForEach(delegate (KeyValuePair<Pk, ImportPk> obj)
            {
                Type t = obj.Key.GetType();
                PropertyInfo[] properties = t.GetProperties();
                string[] fields = new string[properties.Length];
                int i = 0;
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetValue(obj.Key, null) != null)
                    {
                        fields[i] = property.GetValue(obj.Key, null).ToString();

                    }
                    else
                        fields[i] = "";
                    i++;
                }
                dtEmp1.Rows.Add(fields);

                Type t2 = obj.Value.GetType();
                PropertyInfo[] properties2 = t2.GetProperties();
                string[] fields2 = new string[properties2.Length];
                i = 0;
                foreach (PropertyInfo property in properties2)
                {
                    if (property.GetValue(obj.Value, null) != null)
                    {
                        fields2[i] = property.GetValue(obj.Value, null).ToString();
                    }
                    else
                    {
                        fields2[i] = "";
                    }
                    i++;
                }
                dtEmp2.Rows.Add(fields2);
            });
            dataGridView1.DataSource = dtEmp1;
            dataGridView2.DataSource = dtEmp2;

            System.Threading.Thread t = new System.Threading.Thread(InitilizeColors);
            t.Start();

        }

        private void FillTablesPl()
        {
            DataTable dtEmp1 = new DataTable();
            DataTable dtEmp2 = new DataTable();
            // add column to datatable  
            int count = typeof(Pl).GetProperties().Length;
            int count2 = typeof(ImportPl).GetProperties().Length;
            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "Chk";
            dgvCmb.FalseValue = false;
            dgvCmb.IndeterminateValue = false;
            dgvCmb.HeaderText = "CheckBox";
            dataGridView1.Columns.Add(dgvCmb);

            DataGridViewCheckBoxColumn dgvCmb2 = new DataGridViewCheckBoxColumn();
            dgvCmb2.ValueType = typeof(bool);
            dgvCmb2.Name = "Chk";
            dgvCmb2.FalseValue = false;
            dgvCmb2.HeaderText = "CheckBox";
            dataGridView2.Columns.Add(dgvCmb2);

            for (int i = 0; i < count; i++)
            {
                dtEmp1.Columns.Add("data" + i, typeof(string));
            }
            for (int i = 0; i < count2; i++)
            {
                dtEmp2.Columns.Add("dataaa" + i, typeof(string));
            }
            conflictsPl.ForEach(delegate (KeyValuePair<Pl, ImportPl> obj)
            {
                Type t = obj.Key.GetType();
                PropertyInfo[] properties = t.GetProperties();
                string[] fields = new string[properties.Length];
                int i = 0;
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetValue(obj.Key, null) != null)
                    {
                        fields[i] = property.GetValue(obj.Key, null).ToString();

                    }
                    else
                        fields[i] = "";
                    i++;
                }
                dtEmp1.Rows.Add(fields);

                Type t2 = obj.Value.GetType();
                PropertyInfo[] properties2 = t2.GetProperties();
                string[] fields2 = new string[properties2.Length];
                i = 0;
                foreach (PropertyInfo property in properties2)
                {
                    if (property.GetValue(obj.Value, null) != null)
                    {
                        fields2[i] = property.GetValue(obj.Value, null).ToString();
                    }
                    else
                    {
                        fields2[i] = "";
                    }
                    i++;
                }
                dtEmp2.Rows.Add(fields2);
            });
            dataGridView1.DataSource = dtEmp1;
            dataGridView2.DataSource = dtEmp2;

            System.Threading.Thread t = new System.Threading.Thread(InitilizeColors);
            t.Start();

        }

        private void FillTablesPs()
        {
            DataTable dtEmp1 = new DataTable();
            DataTable dtEmp2 = new DataTable();
            // add column to datatable  
            int count = typeof(Ps).GetProperties().Length;
            int count2 = typeof(ImportPs).GetProperties().Length;
            DataGridViewCheckBoxColumn dgvCmb = new DataGridViewCheckBoxColumn();
            dgvCmb.ValueType = typeof(bool);
            dgvCmb.Name = "Chk";
            dgvCmb.FalseValue = false;
            dgvCmb.IndeterminateValue = false;
            dgvCmb.HeaderText = "CheckBox";
            dataGridView1.Columns.Add(dgvCmb);

            DataGridViewCheckBoxColumn dgvCmb2 = new DataGridViewCheckBoxColumn();
            dgvCmb2.ValueType = typeof(bool);
            dgvCmb2.Name = "Chk";
            dgvCmb2.FalseValue = false;
            dgvCmb2.HeaderText = "CheckBox";
            dataGridView2.Columns.Add(dgvCmb2);

            for (int i = 0; i < count; i++)
            {
                dtEmp1.Columns.Add("data" + i, typeof(string));
            }
            for (int i = 0; i < count2; i++)
            {
                dtEmp2.Columns.Add("dataaa" + i, typeof(string));
            }
            conflictsPs.ForEach(delegate (KeyValuePair<Ps, ImportPs> obj)
            {
                Type t = obj.Key.GetType();
                PropertyInfo[] properties = t.GetProperties();
                string[] fields = new string[properties.Length];
                int i = 0;
                foreach (PropertyInfo property in properties)
                {
                    if (property.GetValue(obj.Key, null) != null)
                    {
                        fields[i] = property.GetValue(obj.Key, null).ToString();

                    }
                    else
                        fields[i] = "";
                    i++;
                }
                dtEmp1.Rows.Add(fields);

                Type t2 = obj.Value.GetType();
                PropertyInfo[] properties2 = t2.GetProperties();
                string[] fields2 = new string[properties2.Length];
                i = 0;
                foreach (PropertyInfo property in properties2)
                {
                    if (property.GetValue(obj.Value, null) != null)
                    {
                        fields2[i] = property.GetValue(obj.Value, null).ToString();
                    }
                    else
                    {
                        fields2[i] = "";
                    }
                    i++;
                }
                dtEmp2.Rows.Add(fields2);
            });
            dataGridView1.DataSource = dtEmp1;
            dataGridView2.DataSource = dtEmp2;

            System.Threading.Thread t = new System.Threading.Thread(InitilizeColors);
            t.Start();

        }

        private void InitilizeColors()
        {
            System.Threading.Thread.Sleep(1000);
            int index = 0;
            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                DataGridViewCheckBoxCell chkchecking = dataGridView2["Chk", index] as DataGridViewCheckBoxCell;
                chkchecking.Value = true;
                index++;
            }
        }

        private void Merge_click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Möchten Sie die Änderungen speichern ?", "Title", MessageBoxButtons.YesNo,
                MessageBoxIcon.Information);
            
            if (dr == DialogResult.Yes)
            {
                
                switch (mergeCount)
                {
                    case 1:
                        {
                            int rowCounter = 0;
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (dataGridView1.Rows[rowCounter].DefaultCellStyle.BackColor == Color.PaleVioletRed)
                                {
                                    if (dataGridView1["data0", rowCounter].Value != null)
                                        removedOriginalDataPp.Add(dataGridView1["data0", rowCounter].Value.ToString());
                                }
                                rowCounter++;
                            }
                            rowCounter = 0;
                            foreach (DataGridViewRow row in dataGridView2.Rows)
                            {
                                if (dataGridView2.Rows[rowCounter].DefaultCellStyle.BackColor == Color.PaleVioletRed)
                                {
                                    if (dataGridView2["dataaa0", rowCounter].Value != null)
                                        removednewDataPp.Add(row.Cells["dataaa0"].Value.ToString());
                                }
                                DataGridViewCheckBoxCell chkchecking = dataGridView2["Chk", rowCounter] as DataGridViewCheckBoxCell;
                                rowCounter++;
                            }

                            if (this.conflictsPh.Count > 0)
                            {
                                FillTablesPh();
                                titleLabel.Text = "Konflikt Ph";
                                mergeCount = 2;
                            }
                            else if (conflictsPk.Count > 0)
                            {
                                FillTablesPk();
                                titleLabel.Text = "Konflikt Pk";
                                mergeCount = 3;
                            }
                            else if (conflictsPl.Count > 0)
                            {
                                FillTablesPl();
                                titleLabel.Text = "Konflikt Pl";
                                mergeCount = 4;
                            }
                            else if (conflictsPs.Count > 0)
                            {
                                FillTablesPs();
                                titleLabel.Text = "Konflikt Ps";
                                mergeCount = 5;
                            }
                            break;
                        }
                    case 2:
                        {
                            int rowCounter = 0;
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (dataGridView1.Rows[rowCounter].DefaultCellStyle.BackColor == Color.PaleVioletRed)
                                {
                                    if (dataGridView1["data0", rowCounter].Value != null)
                                        removedOriginalDataPh.Add(dataGridView1["data0", rowCounter].Value.ToString());
                                }
                                rowCounter++;
                            }
                            rowCounter = 0;
                            foreach (DataGridViewRow row in dataGridView2.Rows)
                            {
                                if (dataGridView2.Rows[rowCounter].DefaultCellStyle.BackColor == Color.PaleVioletRed)
                                {
                                    if (dataGridView2["dataaa0", rowCounter].Value != null)
                                        removednewDataPh.Add(row.Cells["dataaa0"].Value.ToString());
                                }
                                DataGridViewCheckBoxCell chkchecking = dataGridView2["Chk", rowCounter] as DataGridViewCheckBoxCell;
                                rowCounter++;
                            }
                            if (conflictsPk.Count > 0)
                            {
                                FillTablesPk();
                                titleLabel.Text = "Konflikt Pk";
                                mergeCount = 3;
                            }
                            else if (conflictsPl.Count > 0)
                            {
                                FillTablesPl();
                                titleLabel.Text = "Konflikt Pl";
                                mergeCount = 4;
                            }
                            else if (conflictsPs.Count > 0)
                            {
                                FillTablesPs();
                                titleLabel.Text = "Konflikt Ps";
                                mergeCount = 5;
                            }
                            break;
                        }
                    case 3:
                        {
                            int rowCounter = 0;
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (dataGridView1.Rows[rowCounter].DefaultCellStyle.BackColor == Color.PaleVioletRed)
                                {
                                    if (dataGridView1["data0", rowCounter].Value != null)
                                        removedOriginalDataPk.Add(dataGridView1["data0", rowCounter].Value.ToString());
                                }
                                rowCounter++;
                            }
                            rowCounter = 0;
                            foreach (DataGridViewRow row in dataGridView2.Rows)
                            {
                                if (dataGridView2.Rows[rowCounter].DefaultCellStyle.BackColor == Color.PaleVioletRed)
                                {
                                    if (dataGridView2["dataaa0", rowCounter].Value != null)
                                        removednewDataPk.Add(row.Cells["dataaa0"].Value.ToString());
                                }
                                DataGridViewCheckBoxCell chkchecking = dataGridView2["Chk", rowCounter] as DataGridViewCheckBoxCell;
                                rowCounter++;
                            }
                            if (conflictsPl.Count > 0)
                            {
                                FillTablesPl();
                                titleLabel.Text = "Konflikt Pl";
                                mergeCount = 4;
                            }
                            else if (conflictsPs.Count > 0)
                            {
                                FillTablesPs();
                                titleLabel.Text = "Konflikt Ps";
                                mergeCount = 5;
                            }
                            break;
                        }
                    case 4:
                        {
                            int rowCounter = 0;
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (dataGridView1.Rows[rowCounter].DefaultCellStyle.BackColor == Color.PaleVioletRed)
                                {
                                    if (dataGridView1["data0", rowCounter].Value != null)
                                        removedOriginalDataPl.Add(dataGridView1["data0", rowCounter].Value.ToString());
                                }
                                rowCounter++;
                            }
                            rowCounter = 0;
                            foreach (DataGridViewRow row in dataGridView2.Rows)
                            {
                                if (dataGridView2.Rows[rowCounter].DefaultCellStyle.BackColor == Color.PaleVioletRed)
                                {
                                    if (dataGridView2["dataaa0", rowCounter].Value != null)
                                        removednewDataPl.Add(row.Cells["dataaa0"].Value.ToString());
                                }
                                DataGridViewCheckBoxCell chkchecking = dataGridView2["Chk", rowCounter] as DataGridViewCheckBoxCell;
                                rowCounter++;
                            }
                            if (conflictsPs.Count > 0)
                            {
                                FillTablesPs();
                                titleLabel.Text = "Konflikt Ps";
                                mergeCount = 5;
                            }
                            break;
                        }
                    case 5:
                        {
                            int rowCounter = 0;
                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (dataGridView1.Rows[rowCounter].DefaultCellStyle.BackColor == Color.PaleVioletRed)
                                {
                                    if (dataGridView1["data0", rowCounter].Value != null)
                                        removedOriginalDataPs.Add(dataGridView1["data0", rowCounter].Value.ToString());
                                }
                                rowCounter++;
                            }
                            rowCounter = 0;
                            foreach (DataGridViewRow row in dataGridView2.Rows)
                            {
                                if (dataGridView2.Rows[rowCounter].DefaultCellStyle.BackColor == Color.PaleVioletRed)
                                {
                                    if (dataGridView2["dataaa0", rowCounter].Value != null)
                                        removednewDataPs.Add(row.Cells["dataaa0"].Value.ToString());
                                }
                                DataGridViewCheckBoxCell chkchecking = dataGridView2["Chk", rowCounter] as DataGridViewCheckBoxCell;
                                rowCounter++;
                            }
                            mergeCount++;
                            break;
                        }
                }

            }

            #region Removed lists Debug
            System.Diagnostics.Debug.WriteLine("Prining removed original Data (Pp) : ");
            foreach (string PAD in removedOriginalDataPp) 
            {
                System.Diagnostics.Debug.WriteLine(PAD);
            }
            System.Diagnostics.Debug.WriteLine("\n\nPrining removed new Data (Pp) : ");
            foreach (string PAD in removednewDataPp)
            {
                System.Diagnostics.Debug.WriteLine(PAD);
            }
            System.Diagnostics.Debug.WriteLine("Prining removed original Data (Ph) : ");
            foreach (string PAD in removedOriginalDataPh)
            {
                System.Diagnostics.Debug.WriteLine(PAD);
            }
            System.Diagnostics.Debug.WriteLine("\n\nPrining removed new Data (Ph) : ");
            foreach (string PAD in removednewDataPh)
            {
                System.Diagnostics.Debug.WriteLine(PAD);
            }
            System.Diagnostics.Debug.WriteLine("Prining removed original Data (Pk) : ");
            foreach (string PAD in removedOriginalDataPk)
            {
                System.Diagnostics.Debug.WriteLine(PAD);
            }
            System.Diagnostics.Debug.WriteLine("\n\nPrining removed new Data (Pk) : ");
            foreach (string PAD in removednewDataPk)
            {
                System.Diagnostics.Debug.WriteLine(PAD);
            }
            System.Diagnostics.Debug.WriteLine("Prining removed original Data (Pl) : ");
            foreach (string PAD in removedOriginalDataPl)
            {
                System.Diagnostics.Debug.WriteLine(PAD);
            }
            System.Diagnostics.Debug.WriteLine("\n\nPrining removed new Data (Pl) : ");
            foreach (string PAD in removednewDataPl)
            {
                System.Diagnostics.Debug.WriteLine(PAD);
            }
            System.Diagnostics.Debug.WriteLine("Prining removed original Data (Ps) : ");
            foreach (string PAD in removedOriginalDataPs)
            {
                System.Diagnostics.Debug.WriteLine(PAD);
            }
            System.Diagnostics.Debug.WriteLine("\n\nPrining removed new Data (Ps) : ");
            foreach (string PAD in removednewDataPs)
            {
                System.Diagnostics.Debug.WriteLine(PAD);
            }

            
            #endregion
        }

        private void dataGridView1_changed_value(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCheckBoxCell chkchecking = dataGridView1["Chk", e.RowIndex] as DataGridViewCheckBoxCell;
            if (Convert.ToBoolean(chkchecking.Value) == true)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.PaleVioletRed;
                DataGridViewCheckBoxCell check = dataGridView2["Chk", e.RowIndex] as DataGridViewCheckBoxCell;
                check.Value = false;
            }
            else
            {
                dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.PaleVioletRed;
                DataGridViewCheckBoxCell check = dataGridView2["Chk", e.RowIndex] as DataGridViewCheckBoxCell;
                check.Value = true;
            }
        }

        private void dataGridView1_cell_clicked(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dataGridView2_cell_clicked(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dataGridView2_changed_value(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCheckBoxCell chkchecking = dataGridView2["Chk", e.RowIndex] as DataGridViewCheckBoxCell;
            if (Convert.ToBoolean(chkchecking.Value) == true)
            {
                dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.PaleVioletRed;
                DataGridViewCheckBoxCell check = dataGridView1["Chk", e.RowIndex] as DataGridViewCheckBoxCell;
                check.Value = false;
            }
            else
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                dataGridView2.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.PaleVioletRed;
                DataGridViewCheckBoxCell check = dataGridView1["Chk", e.RowIndex] as DataGridViewCheckBoxCell;
                check.Value = true;
            }
        }

        private void SaveWithoutConflictsButton_Click(object sender, EventArgs e)
        {
            if (removednewDataPp.Count > 0 || removednewDataPp.Count > 0)
            {
                Import.DeleteUnwantedDataFromTables(removednewDataPp, removedOriginalDataPp, MainWindow.DbGlobal, "Pp");
            }
            if (removednewDataPh.Count > 0 || removednewDataPh.Count > 0)
            {
                Import.DeleteUnwantedDataFromTables(removednewDataPh, removedOriginalDataPh, MainWindow.DbGlobal, "Ph");
            }
            if (removednewDataPk.Count > 0 || removednewDataPk.Count > 0)
            {
                Import.DeleteUnwantedDataFromTables(removednewDataPk, removedOriginalDataPk, MainWindow.DbGlobal, "Pk");
            }
            if (removednewDataPl.Count > 0 || removednewDataPl.Count > 0)
            {
                Import.DeleteUnwantedDataFromTables(removednewDataPl, removedOriginalDataPl, MainWindow.DbGlobal, "Pl");
            }
            if (removednewDataPs.Count > 0 || removednewDataPs.Count > 0)
            {
                Import.DeleteUnwantedDataFromTables(removednewDataPs, removedOriginalDataPs, MainWindow.DbGlobal, "Ps");
            }
            Import.SaveDataInDataBase(MainWindow.DbGlobal, MainWindow.dataTableforTemp, MainWindow.text);
            MessageBox.Show("Die Datensätze sind gespeichert", "Info");
            Close();            
        }
        
        /*private void abbrechen_Button_Click(object sender, EventArgs e)
        {
            removednewDataPp.Clear();
            removednewDataPh.Clear();
            removednewDataPk.Clear();
            removednewDataPl.Clear();
            removednewDataPs.Clear();
            removedOriginalDataPp.Clear();
            removedOriginalDataPh.Clear();
            removedOriginalDataPk.Clear();
            removedOriginalDataPl.Clear();
            removedOriginalDataPs.Clear();
            conflictsPp.Clear();
            conflictsPh.Clear();
            conflictsPk.Clear();
            conflictsPl.Clear(); 
            conflictsPs.Clear();
            this.Visible = false;

        }*/

        private void Abbrechen_button_Click(object sender, EventArgs e)
        {
            removednewDataPp.Clear();
            removednewDataPh.Clear();
            removednewDataPk.Clear();
            removednewDataPl.Clear();
            removednewDataPs.Clear();
            removedOriginalDataPp.Clear();
            removedOriginalDataPh.Clear();
            removedOriginalDataPk.Clear();
            removedOriginalDataPl.Clear();
            removedOriginalDataPs.Clear();
            conflictsPp.Clear();
            conflictsPh.Clear();
            conflictsPk.Clear();
            conflictsPl.Clear();
            conflictsPs.Clear();
            this.Visible = false;

        }
    }
}
