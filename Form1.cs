﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PollingSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void lblCreatePoll_Click(object sender, EventArgs e)
        {

        }

        private void btnAddChoice_Click(object sender, EventArgs e)
        {
            string choice = txtChoice.Text.Trim();
            if (!string.IsNullOrEmpty(choice))
            {
                lstChoices.Items.Add(choice);
                txtChoice.Clear();
            }
        }

        private void btnCreatePoll_Click(object sender, EventArgs e)
        {
            string question = txtPollQuestion.Text.Trim();
            if (!string.IsNullOrEmpty(question) && lstChoices.Items.Count > 0)
            {
                Poll poll = new Poll(question);
                foreach (string choice in lstChoices.Items)
                {
                    poll.AddChoice(choice);
                }
                PollManager.Polls.Add(poll);
                cmbPolls.Items.Add(question);
                MessageBox.Show("Poll created successfully!");

                // Clear inputs
                txtPollQuestion.Clear();
                lstChoices.Items.Clear();
            }
        }

        private void cmbPolls_SelectedIndexChanged(object sender, EventArgs e)
        {
            grpChoices.Controls.Clear();
            int selectedIndex = cmbPolls.SelectedIndex;
            if (selectedIndex >= 0)
            {
                Poll selectedPoll = PollManager.Polls[selectedIndex];
                int y = 20;
                foreach (string choice in selectedPoll.Choices)
                {
                    RadioButton rb = new RadioButton
                    {
                        Text = choice,
                        Location = new System.Drawing.Point(10, y),
                        AutoSize = true
                    };
                    grpChoices.Controls.Add(rb);
                    y += 30;
                }
            }
        }

        private void btnVote_Click(object sender, EventArgs e)
        {
            int selectedIndex = cmbPolls.SelectedIndex;
            if (selectedIndex >= 0)
            {
                Poll selectedPoll = PollManager.Polls[selectedIndex];
                foreach (RadioButton rb in grpChoices.Controls)
                {
                    if (rb.Checked)
                    {
                        selectedPoll.Vote(rb.Text);
                        MessageBox.Show("Vote submitted successfully!");
                        break;
                    }
                }
            }
        }

        private void lblResult_Click(object sender, EventArgs e)
        {
            int selectedIndex = cmbPolls.SelectedIndex;
            if (selectedIndex >= 0)
            {
                Poll selectedPoll = PollManager.Polls[selectedIndex];
                lblResult.Text = selectedPoll.GetResults();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
