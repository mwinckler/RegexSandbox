using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace RegexSandbox {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
			this.Loaded += (sndr, e) => {
				txtRegex.KeyUp += txtInput_KeyUp;
				txtSubjectText.KeyUp += txtInput_KeyUp;
				chkAutoUpdate.Checked += chkAutoUpdate_Checked;
			};

		}

		private void btnMatch_Click(object sender, RoutedEventArgs e) {
			UpdateResults();
		}

		private void UpdateResults() {
			treeResult.Items.Clear();
			if (string.IsNullOrEmpty(txtRegex.Text))
				return;

			Regex regex = null;
			try {
				regex = new Regex(txtRegex.Text);
			} catch (ArgumentException ex) {
				treeResult.Items.Add("Syntax error " + ex.Message);
				return;
			}
			var matches = regex.Matches(txtSubjectText.Text);

			if (matches.Count == 0) {
				treeResult.Items.Add("(no matches found)");
			} else {
				int matchIndex = 0;
				foreach (Match match in matches) {
					var parent = new TreeViewItem() {
						Header = "Match " + matchIndex++,
						IsExpanded = true
					};

					if (match.Success) {
						parent.Items.Add(GetCaptures(match.Captures));

						int i = 0;
						var groups = new TreeViewItem() { 
							Header = "Groups",
							IsExpanded = true
						};
						foreach (Group group in match.Groups) {

							var node = new TreeViewItem() {
								Header = string.Format(
											"Group {0}{1}: {2}",
											i,
											(regex.GroupNameFromNumber(i) != i.ToString() ? " (?<" + regex.GroupNameFromNumber(i) + ">)" : ""),
											(group.Success ? group.Value : "(Not matched)")
										),
								IsExpanded = true
							};

							groups.Items.Add(node);
							i++;
						}

						parent.Items.Add(groups);
					}
					treeResult.Items.Add(parent);
					
				}

			}
		}

		private TreeViewItem GetCaptures(CaptureCollection captures) {
			TreeViewItem parent = new TreeViewItem() { Header = "Captures" };
			int i = 0;
			foreach (Capture cap in captures) {
				var node = new TreeViewItem() {
					Header = string.Format("Capture {0}: '{1}'", i++, cap.ToString()),
					IsExpanded = true
				};
				node.Items.Add("Index: " + cap.Index);
				node.Items.Add("Length: " + cap.Length);
				node.Items.Add("Value: " + cap.Value);
				parent.Items.Add(node);
			}
			return parent;
		}

		private void chkAutoUpdate_Checked(object sender, RoutedEventArgs e) {
			if (chkAutoUpdate.IsChecked ?? false) {
				UpdateResults();
			}
		}

		private void txtInput_KeyUp(object sender, KeyEventArgs e) {
			if (chkAutoUpdate.IsChecked ?? false) {
				UpdateResults();
			}
		}

	}

}
