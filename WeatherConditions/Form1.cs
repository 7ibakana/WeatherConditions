using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeatherConditions
{
    public partial class Form1 : Form
    {
        //global variable with server's address
        readonly string BaseUrl = "http://weather-csharp.herokuapp.com/";
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetWeather_Click(object sender, EventArgs e)
        {
            //disable button to preven user making another request before this one is done
            btnGetWeather.Enabled = false;

            //read data from TextBoxes
            string city = txtCity.Text;
            string state = txtState.Text;

            if (LocationDataValid(city, state))
            {
                //Fetch current weather and display
                if (GetWeatherText(city, state, out string weather, out string textErrorMessage))
                {
                    lblWeather.Text = weather;
                }
                else
                {
                    MessageBox.Show(textErrorMessage, "Error");
                }
                if (picWeather.Image !=null)
                {
                    picWeather.Image.Dispose(); //clear previous image
                }
                if (GetWeatherImage(city, state, out Image image, out string imageErrorMessage))
                {
                    picWeather.Image = image;
                }
            }
            else
            {
                MessageBox.Show("Enter both city and state", "Error");
            }
            //Enable button so user can get weather for somewhere else
            btnGetWeather.Enabled = true;
        }
        private bool GetWeatherText(string city, string state, out string weatherText, out string errorMessage)
        {
            //use the Format method to make a string in the format
            // http://weather-csharp.herokuapp.com/text?city=dalls&state=tx

            string weatherTextUrl = String.Format("{0}text?city={1}&state={2}", BaseUrl, city, state);
            Debug.WriteLine(weatherTextUrl); //message only seen by developers

            errorMessage = null;
            weatherText = null;
            try
            {
                using (WebClient client = new WebClient())
                {
                    weatherText = client.DownloadString(weatherTextUrl);
                }
                Debug.WriteLine(weatherText);
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
                errorMessage = e.Message; //you'll refine this in lab
                return false;
            }
        }
        private bool LocationDataValid(string city, string state)
        {
            //make checks on data, return false if any fail
            if (String.IsNullOrWhiteSpace(city))
            {
                return false;
            }
            if (String.IsNullOrWhiteSpace(state))
            {
                return false;
            }
            if (city.Any(char.IsDigit)) //added to validate that an error is given when a digit is entered
            {
                return false;
            }
            //all checks passed? data looks good, return true
            return true;

        }
        private bool GetWeatherImage(string city, string state, out Image weatherImage, out string errorMessage)
        {
            weatherImage = null; //initialize the out parameters
            errorMessage = null; //will only set one of these, depending on if things work or not
            
            try
            {
                using (WebClient client = new WebClient())
                {
                    //use the format method to make a string in the format
                    //http://weather-csharp.herokuapp.com/photo?city=austin&state=tx
                    string weatherPhotoUrl = String.Format("{0}photo?city={1}&state={2}", BaseUrl, city, state);
                    string tempFileDirectory = Path.GetTempPath().ToString(); //directory to save image
                    String weatherFilePath = Path.Combine(tempFileDirectory, "weather_image.jpeg");
                    Debug.WriteLine(weatherFilePath);
                    client.DownloadFile(weatherPhotoUrl, weatherFilePath); //download from URL
                    weatherImage = Image.FromFile(weatherFilePath); //setting the out parameter
                }
                return true; //request was made, file was save, no errors
            }
             catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace); //to help troubleshoot
                errorMessage = e.Message; //setting the out parameter
                return false; //to inform the caller that there was an error
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close(); //Closes app
        }
    }
}
