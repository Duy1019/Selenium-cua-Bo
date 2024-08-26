using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using Demo01_UploadFile;

[assembly: Parallelizable(ParallelScope.Fixtures)]

namespace Selenium
{
	[TestFixture("Chrome", "127.0", "Windows 11", "Chrome127")]
	[TestFixture("Safari", "17.0", "MacOS Sonoma", "Safari17")]
	public class Testing : BaseTest_Lambda
	{

		public Testing(string browser, string version, string os, string name) : base(browser, version, os, name)
		{

		}

		private string GetFilePathBasedOnOS(string os, string fileName)
		{
			string filePath;

			if (os.Contains("MacOS"))
			{
				// macOS
				filePath = $"/Users/ltuser/Downloads/{fileName}";
			}
			else if (os.Contains("Windows"))
			{
				// Windows
				filePath = $@"C:\Users\ltuser\Downloads\{fileName}";
			}
			else
			{
				throw new PlatformNotSupportedException("Unsupported OS: " + os);
			}

			return filePath;
		}

		[TestCase("BoNguyen", "Vanbo2003+", "http://localhost:22220/")] // Trường hợp đăng nhập thành công
		[TestCase("wrongemail@gmail.com", "wrongpassword", "http://localhost:22220/account/login")] // Trường hợp đăng nhập không thành công
		[TestCase("", "", "http://localhost:22220/account/login")] // Trường hợp bỏ trống các field
		public void Dang_Nhap(string userName, string password, string expectedUrl)
		{
			_driver.Navigate().GoToUrl("http://localhost:22220/account/login");

			// Xác định vị trí trường tên người dùng và nhập tên người dùng
			var userNameField = _driver.FindElement(By.Name("UserName"));
			userNameField.Clear(); // Xóa giá trị trước đó (nếu có)
			userNameField.SendKeys(userName);

			// Xác định vị trí trường mật khẩu và nhập mật khẩu
			var passwordField = _driver.FindElement(By.Name("Password"));
			passwordField.Clear(); // Xóa giá trị trước đó (nếu có)
			passwordField.SendKeys(password);

			// Xác định vị trí nút đăng nhập và nhấp vào
			var loginButton = _driver.FindElement(By.XPath("//button[text()='Login']"));
			loginButton.Click();

			if (expectedUrl == "http://localhost:22220/")
			{
				wait.Until(driver => driver.Url == expectedUrl);
				Assert.That(_driver.Url, Is.EqualTo(expectedUrl), "URL không khớp với trang mong đợi sau khi đăng nhập.");
			}
			else
			{
				// Kiểm tra sự xuất hiện của thông báo lỗi cho từng trường
				if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password))
				{
					// Kiểm tra các thông báo lỗi riêng lẻ
					var userNameError = wait.Until(driver => driver.FindElement(By.CssSelector("input[name='UserName'] + .text-danger.field-validation-error")));
					var passwordError = wait.Until(driver => driver.FindElement(By.CssSelector("input[name='Password'] + .text-danger.field-validation-error")));

					Assert.IsTrue(userNameError.Displayed, "Không tìm thấy thông báo lỗi cho trường UserName.");
					Assert.IsTrue(passwordError.Displayed, "Không tìm thấy thông báo lỗi cho trường Password.");
				}
				else
				{
					// Kiểm tra sự xuất hiện của thông báo lỗi chung
					var errorElement = wait.Until(driver => driver.FindElement(By.CssSelector(".text-danger.validation-summary-errors")));
					Assert.IsTrue(errorElement.Displayed, "Không tìm thấy thông báo lỗi sau khi đăng nhập không thành công.");
				}
			}
		}


		[TestCase("validuser22", "validuser22@example.com", "ValidPassword123", "http://localhost:22220/account/login")]
		[TestCase("user2", "user2@example.com", "123456", "http://localhost:22220/Account/Create")] // Mật khẩu chỉ có số
		[TestCase("user3", "user3@example.com", "passwordonly", "http://localhost:22220/Account/Create")] // Mật khẩu chỉ có chữ
		[TestCase("", "", "", "http://localhost:22220/Account/Create")] // Bỏ trống các trường
		[TestCase("duplicateuser", "bonguyen0105@gmail.com", "ValidPassword123", "http://localhost:22220/Account/Create")] // Email đã tồn tại
		public void Dang_Ky(string userName, string email, string password, string expectedUrl)
		{
			_driver.Navigate().GoToUrl("http://localhost:22220/Account/Create");

			// Xác định và điền thông tin vào trường UserName
			var userNameField = _driver.FindElement(By.Name("UserName"));
			userNameField.Clear();
			userNameField.SendKeys(userName);

			// Xác định và điền thông tin vào trường Email
			var emailField = _driver.FindElement(By.Name("Email"));
			emailField.Clear();
			emailField.SendKeys(email);

			// Xác định và điền thông tin vào trường Password
			var passwordField = _driver.FindElement(By.Name("Password"));
			passwordField.Clear();
			passwordField.SendKeys(password);

			// Xác định và nhấp vào nút Đăng ký
			var signupButton = _driver.FindElement(By.XPath("//button[text()='Signup']"));
			signupButton.Click();

			if (expectedUrl == "http://localhost:22220/account/login")
			{
				// Kiểm tra trang đăng nhập
				wait.Until(driver => driver.Url == "http://localhost:22220/account/login");
				Assert.That(_driver.Url, Is.EqualTo(expectedUrl), "URL không khớp với trang đăng nhập sau khi đăng ký.");
			}
			else
			{
				// Kiểm tra thông báo lỗi tùy thuộc vào tình huống

				if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(email) && string.IsNullOrEmpty(password))
				{
					// Kiểm tra các thông báo lỗi cho các trường trống
					var userNameError = wait.Until(driver => driver.FindElement(By.CssSelector("span[data-valmsg-for='UserName']")));
					var emailError = wait.Until(driver => driver.FindElement(By.CssSelector("span[data-valmsg-for='Email']")));
					var passwordError = wait.Until(driver => driver.FindElement(By.CssSelector("span[data-valmsg-for='Password']")));

					Assert.IsTrue(userNameError.Displayed, "Không tìm thấy thông báo lỗi cho trường UserName.");
					Assert.IsTrue(emailError.Displayed, "Không tìm thấy thông báo lỗi cho trường Email.");
					Assert.IsTrue(passwordError.Displayed, "Không tìm thấy thông báo lỗi cho trường Password.");
				}
				else
				{
					// Kiểm tra thông báo lỗi chung
					var errorElement = wait.Until(driver => driver.FindElement(By.CssSelector(".text-danger.validation-summary-errors")));
					Assert.IsTrue(errorElement.Displayed, "Không tìm thấy thông báo lỗi chung.");

					if (password.Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
					{
						// Nếu mật khẩu chỉ có chữ
						Assert.IsTrue(errorElement.Text.Contains("Passwords must have at least one digit ('0'-'9')."), "Thông báo lỗi mật khẩu không chứa số không hiển thị.");
					}
					else if (password.Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]"))
					{
						// Nếu mật khẩu chỉ có số
						Assert.IsTrue(errorElement.Text.Contains("Passwords must have at least one lowercase ('a'-'z')."), "Thông báo lỗi mật khẩu không chứa chữ cái thường không hiển thị.");
					}
					else if (email.Contains("bonguyen0105@gmail.com"))
					{
						// Nếu email đã tồn tại
						Assert.IsTrue(errorElement.Text.Contains("Email 'bonguyen0105@gmail.com' is already taken."), "Thông báo lỗi email đã tồn tại không hiển thị.");
					}
				}
			}
		}

		[TestCase("SanPhamTest", "99.99", "This is a sample product description.", 5, 5, "http://localhost:22220/Admin")]
		[TestCase("", "", "", 0, 0, "http://localhost:22220/Admin/Product/Create")]
		public void Them_SP(string name, string price, string description, int categoryIndex, int brandIndex, string expectedUrl)
		{
			_driver.Navigate().GoToUrl("http://localhost:22220/Admin/Product/Create");

			// Fill in the fields if provided
			if (!string.IsNullOrEmpty(name))
				_driver.FindElement(By.Name("Name")).SendKeys(name + DateTime.Now);

			if (!string.IsNullOrEmpty(price))
				_driver.FindElement(By.Name("Price")).SendKeys(price.ToString());

			if (!string.IsNullOrEmpty(description))
				_driver.FindElement(By.Name("Description")).SendKeys(description);

			if (categoryIndex > 0)
				new SelectElement(_driver.FindElement(By.Name("CategoryId"))).SelectByIndex(categoryIndex);

			if (brandIndex > 0)
				new SelectElement(_driver.FindElement(By.Name("BrandId"))).SelectByIndex(brandIndex);

			string imgFile = GetFilePathBasedOnOS(_os, "realmiphone.jfif");
			if (!string.IsNullOrEmpty(imgFile))
			{
				var fileInput = _driver.FindElement(By.Name("ImageUpload"));
				fileInput.SendKeys(imgFile);
			}

			// Submit the form
			var submitButton = _driver.FindElement(By.XPath("//button[text()='Create']"));
			submitButton.Click();
			// Handle assertions based on expected behavior
			if (expectedUrl == "http://localhost:22220/Admin")
			{
				// Wait for the redirect to Admin page
				wait.Until(driver => driver.Url == expectedUrl);

				// Assert successful redirection
				Assert.That(_driver.Url, Is.EqualTo(expectedUrl));
			}
			else
			{
				// Validate error messages
				var nameError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Name'].text-danger"));
				var priceError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Price'].text-danger"));
				var descriptionError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Description'].text-danger"));
				var categoryError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='CategoryId'].text-danger"));
				var brandError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='BrandId'].text-danger"));

				// Assert that error messages are displayed
				Assert.IsTrue(nameError.Displayed);
				Assert.IsTrue(priceError.Displayed);
				Assert.IsTrue(descriptionError.Displayed);
				Assert.IsTrue(categoryError.Displayed);
				Assert.IsTrue(brandError.Displayed);

				// Print out the error messages
				Console.WriteLine("Name Error: " + nameError.Text);
				Console.WriteLine("Price Error: " + priceError.Text);
				Console.WriteLine("Description Error: " + descriptionError.Text);
				Console.WriteLine("Category Error: " + categoryError.Text);
				Console.WriteLine("Brand Error: " + brandError.Text);
			}
		}

		[TestCase("Sample Product1", "99.99", "This is a sample product description.", 5, 5, "http://localhost:22220/Admin")]
		[TestCase("", "", "", 0, 0, "http://localhost:22220/Admin/Product/Edit/15")]
		public void Edit_SP(string name, string price, string description, int categoryIndex, int brandIndex, string expectedUrl)
		{
			try
			{
				_driver.Navigate().GoToUrl("http://localhost:22220/Admin/Product/Edit/15");
			}
			catch (Exception e)
			{
				Console.WriteLine("Không tồn tại sản phẩm này. Lỗi: " + e.Message);
			}


			// Fill in the fields if provided
			var nameField = _driver.FindElement(By.Name("Name"));
			nameField.Clear();
			nameField.SendKeys(name + DateTime.Now);

			var priceField = _driver.FindElement(By.Name("Price"));
			priceField.Clear();
			priceField.SendKeys(price.ToString());

			var desField = _driver.FindElement(By.Name("Description"));
			desField.Clear();
			desField.SendKeys(description);

			if (categoryIndex > 0)
				new SelectElement(_driver.FindElement(By.Name("CategoryId"))).SelectByIndex(categoryIndex);

			if (brandIndex > 0)
				new SelectElement(_driver.FindElement(By.Name("BrandId"))).SelectByIndex(brandIndex);

			string imgFile = GetFilePathBasedOnOS(_os, "macbook-air-m1-2020-gray-600x600.jpg");
			if (!string.IsNullOrEmpty(imgFile))
			{
				var fileInput = _driver.FindElement(By.Name("ImageUpload"));
				fileInput.SendKeys(imgFile);
			}

			// Submit the form
			var submitButton = _driver.FindElement(By.XPath("//button[text()='Update']"));
			submitButton.Click();
			// Handle assertions based on expected behavior
			if (expectedUrl == "http://localhost:22220/Admin")
			{
				// Wait for the redirect to Admin page
				wait.Until(driver => driver.Url == expectedUrl);

				// Assert successful redirection
				Assert.That(_driver.Url, Is.EqualTo(expectedUrl));
			}
			else
			{
				// Validate error messages
				var nameError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Name'].text-danger"));
				var priceError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Price'].text-danger"));
				var descriptionError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Description'].text-danger"));
				var categoryError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='CategoryId'].text-danger"));
				var brandError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='BrandId'].text-danger"));

				// Assert that error messages are displayed
				Assert.IsTrue(nameError.Displayed);
				Assert.IsTrue(priceError.Displayed);
				Assert.IsTrue(descriptionError.Displayed);
				Assert.IsTrue(categoryError.Displayed);
				Assert.IsTrue(brandError.Displayed);

				// Print out the error messages
				Console.WriteLine("Name Error: " + nameError.Text);
				Console.WriteLine("Price Error: " + priceError.Text);
				Console.WriteLine("Description Error: " + descriptionError.Text);
				Console.WriteLine("Category Error: " + categoryError.Text);
				Console.WriteLine("Brand Error: " + brandError.Text);
			}
		}

		[TestCase("Sample Category123", "This is a sample category description.", "http://localhost:22220/Admin/Category")]
		[TestCase("", "", "http://localhost:22220/Admin/Category/Create")]
		public void Them_Category(string name, string description, string expectedUrl)
		{
			_driver.Navigate().GoToUrl("http://localhost:22220/Admin/Category/Create");

			// Fill in the fields if provided
			if (!string.IsNullOrEmpty(name))
				_driver.FindElement(By.Name("Name")).SendKeys(name + DateTime.Now);

			if (!string.IsNullOrEmpty(description))
				_driver.FindElement(By.Name("Description")).SendKeys(description);

			// Submit the form
			var submitButton = _driver.FindElement(By.XPath("//button[text()='Create']"));
			submitButton.Click();
			// Handle assertions based on expected behavior
			if (expectedUrl == "http://localhost:22220/Admin/Category")
			{
				// Wait for the redirect to Admin page
				wait.Until(driver => driver.Url == expectedUrl);

				// Assert successful redirection
				Assert.That(_driver.Url, Is.EqualTo(expectedUrl));
			}
			else
			{
				// Validate error messages
				var nameError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Name'].text-danger"));
				var descriptionError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Description'].text-danger"));

				// Assert that error messages are displayed
				Assert.IsTrue(nameError.Displayed);
				Assert.IsTrue(descriptionError.Displayed);

				// Print out the error messages
				Console.WriteLine("Name Error: " + nameError.Text);
				Console.WriteLine("Description Error: " + descriptionError.Text);
			}
		}

		[TestCase("Sample Category123", "This is a sample category description.", "http://localhost:22220/Admin/Category")]
		[TestCase("", "", "http://localhost:22220/Admin/Category/Edit/3")]
		public void Edit_Category(string name, string description, string expectedUrl)
		{
			try
			{
				_driver.Navigate().GoToUrl("http://localhost:22220/Admin/Category/Edit/3");
			}
			catch (Exception e)
			{
				Console.WriteLine("Không tìm thấy Category để Edit. Lỗi: " + e.Message);
			}

			// Fill in the fields if provided
			var nameField = _driver.FindElement(By.Name("Name"));
			nameField.Clear();
			nameField.SendKeys(name + DateTime.Now);

			var desField = _driver.FindElement(By.Name("Description"));
			desField.Clear();
			desField.SendKeys(description);

			// Submit the form
			var submitButton = _driver.FindElement(By.XPath("//button[text()='Update']"));
			submitButton.Click();
			// Handle assertions based on expected behavior
			if (expectedUrl == "http://localhost:22220/Admin/Category")
			{
				// Wait for the redirect to Admin page
				wait.Until(driver => driver.Url == expectedUrl);

				// Assert successful redirection
				Assert.That(_driver.Url, Is.EqualTo(expectedUrl));
			}
			else
			{
				// Validate error messages
				var nameError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Name'].text-danger"));
				var descriptionError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Description'].text-danger"));

				// Assert that error messages are displayed
				Assert.IsTrue(nameError.Displayed);
				Assert.IsTrue(descriptionError.Displayed);

				// Print out the error messages
				Console.WriteLine("Name Error: " + nameError.Text);
				Console.WriteLine("Description Error: " + descriptionError.Text);
			}
		}

		[TestCase("Sample Category123", "This is a sample category description.", "http://localhost:22220/Admin/Brand")]
		[TestCase("", "", "http://localhost:22220/Admin/Brand/Create")]
		public void Them_Brand(string name, string description, string expectedUrl)
		{
			_driver.Navigate().GoToUrl("http://localhost:22220/Admin/Brand/Create");

			// Fill in the fields if provided
			if (!string.IsNullOrEmpty(name))
				_driver.FindElement(By.Name("Name")).SendKeys(name + DateTime.Now);

			if (!string.IsNullOrEmpty(description))
				_driver.FindElement(By.Name("Description")).SendKeys(description);

			// Submit the form
			var submitButton = _driver.FindElement(By.XPath("//button[text()='Create']"));
			submitButton.Click();
			// Handle assertions based on expected behavior
			if (expectedUrl == "http://localhost:22220/Admin/Brand")
			{
				// Wait for the redirect to Admin page
				wait.Until(driver => driver.Url == expectedUrl);

				// Assert successful redirection
				Assert.That(_driver.Url, Is.EqualTo(expectedUrl));
			}
			else
			{
				// Validate error messages
				var nameError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Name'].text-danger"));
				var descriptionError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Description'].text-danger"));

				// Assert that error messages are displayed
				Assert.IsTrue(nameError.Displayed);
				Assert.IsTrue(descriptionError.Displayed);

				// Print out the error messages
				Console.WriteLine("Name Error: " + nameError.Text);
				Console.WriteLine("Description Error: " + descriptionError.Text);
			}
		}

		[TestCase("Sample Category123", "This is a sample category description.", "http://localhost:22220/Admin/Brand")]
		[TestCase("", "", "http://localhost:22220/Admin/Brand/Edit/3")]
		public void Edit_Brand(string name, string description, string expectedUrl)
		{
			try
			{
				_driver.Navigate().GoToUrl("http://localhost:22220/Admin/Brand/Edit/3");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Không tìm thấy Brand để Edit. Lỗi: " + ex.Message);
			}
			// Fill in the fields if provided
			var nameField = _driver.FindElement(By.Name("Name"));
			nameField.Clear();
			nameField.SendKeys(name + DateTime.Now);

			var desField = _driver.FindElement(By.Name("Description"));
			desField.Clear();
			desField.SendKeys(description);

			// Submit the form
			var submitButton = _driver.FindElement(By.XPath("//button[text()='Update']"));
			submitButton.Click();
			// Handle assertions based on expected behavior
			if (expectedUrl == "http://localhost:22220/Admin/Brand")
			{
				// Wait for the redirect to Admin page
				wait.Until(driver => driver.Url == expectedUrl);

				// Assert successful redirection
				Assert.That(_driver.Url, Is.EqualTo(expectedUrl));
			}
			else
			{
				// Validate error messages
				var nameError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Name'].text-danger"));
				var descriptionError = _driver.FindElement(By.CssSelector("span[data-valmsg-for='Description'].text-danger"));

				// Assert that error messages are displayed
				Assert.IsTrue(nameError.Displayed);
				Assert.IsTrue(descriptionError.Displayed);

				// Print out the error messages
				Console.WriteLine("Name Error: " + nameError.Text);
				Console.WriteLine("Description Error: " + descriptionError.Text);
			}
		}
	}
}
