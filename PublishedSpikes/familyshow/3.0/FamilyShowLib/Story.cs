//===============================================================================
// Microsoft patterns & practices
// Composite WPF (PRISM)
//===============================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
//===============================================================================

using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace Microsoft.FamilyShowLib
{
    [Serializable]
    public class Story : INotifyPropertyChanged
    {
        #region Fields and Constants

        // The constants specific to this class
        public static class Const
        {
            // Name of the folder
            public const string StoriesFolderName = "Stories";
        }

        private string relativePath;

        #endregion

        #region Properties

        /// <summary>
        /// The relative path to the story file.
        /// </summary>
        public string RelativePath
        {
            get { return relativePath; }
            set
            {
                if (relativePath != value)
                {
                    relativePath = value;
                    OnPropertyChanged("relativePath");
                }
            }
        }

        /// <summary>
        /// The fully qualified path to the story.
        /// </summary>
        [XmlIgnore]
        public string AbsolutePath
        {
            get
            {
                // Absolute path to the application folder
                string appLocation = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    App.ApplicationFolderName);

                if (relativePath != null)
                    return Path.Combine(appLocation, relativePath);
                else
                    return string.Empty;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Empty constructor is needed for serialization
        /// </summary>
        public Story() { }

        #endregion

        #region Methods

        /// <summary>
        /// Save the story to the file system.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Microsoft.FamilyShowLib.Story.Save(System.String,System.String):System.Void")]
        public void Save(TextRange storyText, string storyFileName)
        {
            // Data format for the story file.
            string dataFormat = DataFormats.Rtf;

            // Absolute path to the application folder
            string appLocation = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                App.ApplicationFolderName);

            // Absolute path to the stories folder
            string storiesLocation = Path.Combine(appLocation, Const.StoriesFolderName);

            // Fully qualified path to the new story file
            storyFileName = GetSafeFileName(storyFileName);
            string storyAbsolutePath = Path.Combine(storiesLocation, storyFileName);

            try
            {
                // Create the stories directory if it doesn't exist
                if (!Directory.Exists(storiesLocation))
                    Directory.CreateDirectory(storiesLocation);

                // Open the file for writing the richtext
                using (FileStream stream = File.Create(storyAbsolutePath))
                {
                    // Store the relative path to the story
                    this.relativePath = Path.Combine(Const.StoriesFolderName, storyFileName);

                    // Save the story to disk
                    if (storyText.CanSave(dataFormat))
                        storyText.Save(stream, dataFormat);
                }
            }
            catch
            {
                // Could not save the story. Handle all exceptions
                // the same, ignore and continue.
            }
        }

        /// <summary>
        /// Load the Story from file to the textrange.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Load(TextRange storyText)
        {
            // Data format for the person's story file.
            string dataFormat = DataFormats.Rtf;

            if (File.Exists(this.AbsolutePath))
            {
                try
                {
                    // Open the file for reading
                    using (FileStream stream = File.OpenRead(this.AbsolutePath))
                    {
                        if (storyText.CanLoad(dataFormat))
                            storyText.Load(stream, dataFormat);
                    }
                }
                catch
                {
                    // Could not load the story. Handle all exceptions
                    // the same, ignore and continue.
                }
            }
        }

        /// <summary>
        /// Save the person's story on the file system.  Accepts plain text for Gedcom support
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Microsoft.FamilyShowLib.Story.Save(System.String,System.String):System.Void")]
        public void Save(string storyText, string storyFileName)
        {
            // Data format for the story file.
            string dataFormat = DataFormats.Rtf;

            // Absolute path to the application folder
            string appLocation = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                App.ApplicationFolderName);

            // Absolute path to the stories folder
            string storiesLocation = Path.Combine(appLocation, Const.StoriesFolderName);

            // Fully qualified path to the new story file
            storyFileName = GetSafeFileName(storyFileName);
            string storyAbsolutePath = Path.Combine(storiesLocation, storyFileName);

            // Convert the text into a TextRange.  This will allow saving the story to disk as RTF.
            TextBlock block = new System.Windows.Controls.TextBlock();
            block.Text = storyText;
            TextRange range = new TextRange(block.ContentStart, block.ContentEnd);

            try
            {
                // Create the stories directory if it doesn't exist
                if (!Directory.Exists(storiesLocation))
                    Directory.CreateDirectory(storiesLocation);

                // Open the file for writing the richtext
                using (FileStream stream = File.Create(storyAbsolutePath))
                {
                    // Store the relative path to the story
                    this.relativePath = Path.Combine(Const.StoriesFolderName, storyFileName);

                    // Save the story to disk
                    if (range.CanSave(dataFormat))
                        range.Save(stream, dataFormat);
                }
            }
            catch
            {
                // Could not save the story. Handle all exceptions
                // the same, ignore and continue.
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Delete()
        {
            // Delete the person's story if it exists
            if (File.Exists(this.AbsolutePath))
            {
                try
                {
                    File.Delete(this.AbsolutePath);
                }
                catch
                {
                    // Could not delete the file. Handle all exceptions
                    // the same, ignore and continue.
                }
            }
        }

        /// <summary>
        /// Return a string that is a safe file name.
        /// </summary>
        private static string GetSafeFileName(string fileName)
        {
            // List of invalid characters.
            char[] invalid = Path.GetInvalidFileNameChars();

            // Remove all invalid characters.
            int pos;
            while ((pos = fileName.IndexOfAny(invalid)) != -1)
                fileName = fileName.Remove(pos, 1);

            return fileName;
        }            

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
