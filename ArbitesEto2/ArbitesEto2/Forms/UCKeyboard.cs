﻿using System;
using System.Collections.Generic;
using System.IO;
using Eto.Forms;


namespace ArbitesEto2
{

    public partial class UCKeyboard
    {

        public Keyboard Keyboard { get; set; }
        public List<UCLayer> Layers { get; set; }
        public string SavePath { get; set; }
        private readonly bool initalized;

        public UCKeyboard()
        {
            InitializeComponent();
        }

        public UCKeyboard(Keyboard keyboard, LayoutContainer layout)
        {
            InitializeComponent();
            this.Keyboard = keyboard;
            this.Layers = new List<UCLayer>();
            this.initalized = true;
            LoadLayout(layout);
            this.SavePath = "<UNSAVED LAYOUT>";
            this.LLayoutName.Text = "<UNSAVED LAYOUT>";
            InitHandle();
        }

        private void InitHandle()
        {
            this.BtnAddLayer.Click += (sender, e) => AddLayer();
            this.BtnSaveAs.Click += (sender, e) => SaveLayoutAs();
            this.BtnSave.Click += (sender, e) => SaveLayout();
            this.BtnLoad.Click += (sender, e) => LoadLayoutFile();
        }

        public void LoadLayoutFile()
        {
            var dialog = new OpenFileDialog();
            dialog.Filters.Add(new FileFilter(this.Keyboard.Name + " layout", "*." + this.Keyboard.SaveFileExtension));
            dialog.Title = "Load Layout";
            dialog.Directory = new Uri(Environment.CurrentDirectory + MdConstant.PathSeparator + "layouts");
            try
            {
                dialog.ShowDialog(this);
                if (!string.IsNullOrEmpty(dialog.FileName))
                {
                    MdSessionData.CurrentLayout = MdCore.DeserializeFromPath<LayoutContainer>(dialog.FileName);
                    LoadLayout(MdSessionData.CurrentLayout);
                    this.SavePath = dialog.FileName;
                    this.LLayoutName.Text = Path.GetFileNameWithoutExtension(this.SavePath);
                    this.DisplaySavedChangedSignal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SaveLayout()
        {
            if (this.SavePath == "<UNSAVED LAYOUT>")
            {
                SaveLayoutAs();
            }
            else
            {
                MdCore.SerializeToPath(MdSessionData.CurrentLayout, this.SavePath);
                this.DisplaySavedChangedSignal();
            }
        }

        public void SaveLayoutAs()
        {
            var dialog = new SaveFileDialog();
            dialog.Filters.Add(new FileFilter(this.Keyboard.Name + " layout", "*." + this.Keyboard.SaveFileExtension));
            dialog.Title = "Save Layout As";
            dialog.Directory = new Uri(Environment.CurrentDirectory + MdConstant.PathSeparator + "layouts");
            try
            {
                dialog.ShowDialog(this);
                if (!string.IsNullOrEmpty(dialog.FileName))
                {
                    this.SavePath = dialog.FileName;

                    // this line is needed because gtk savefiledialog doesnt work properly with extensions
                    this.SavePath = Path.ChangeExtension(dialog.FileName, this.Keyboard.SaveFileExtension);

                    MdCore.SerializeToPath(MdSessionData.CurrentLayout, this.SavePath);
                    this.DisplaySavedChangedSignal();
                    this.LLayoutName.Text = Path.GetFileNameWithoutExtension(this.SavePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void AddLayer()
        {
            if (this.Layers.Count < this.Keyboard.MaxLayers)
            {
                MdSessionData.CurrentLayout.AddLayer(this.Layers.Count);
                LoadLayout(MdSessionData.CurrentLayout);
            }
            else
            {
                MessageBox.Show("Error: Maximum number of layers reached.");
            }
        }

        public void LoadLayout(LayoutContainer input)
        {
            if (this.initalized)
            {
                var cLay = 0;
                foreach (var kd in input.KeyDatas)
                {
                    if (kd.Z > cLay)
                    {
                        cLay = kd.Z;
                    }
                }
                while (this.Layers.Count < cLay + 1)
                {
                    var lay = new UCLayer(this.Keyboard, input, this.Layers.Count);
                    this.Layers.Add(lay);
                    this.SLMain.Items.Add(lay);
                }

                while (this.Layers.Count > cLay + 1)
                {
                    this.SLMain.Items.RemoveAt(this.Layers.Count - 1);
                    this.Layers.RemoveAt(this.Layers.Count - 1);
                }

                foreach (var lay in this.Layers)
                {
                    lay.LoadLayout(input);
                }
                this.DisplayUnsavedChangeSignal();
            }
        }

        public bool Saved { get; private set; } = true;

        public void DisplayUnsavedChangeSignal()
        {
            this.BtnSave.Text = "Save Layout*";
            this.Saved = false;
        }

        public void DisplaySavedChangedSignal()
        {
            this.BtnSave.Text = "Save Layout";
            this.Saved = true;
        }
    }

}
