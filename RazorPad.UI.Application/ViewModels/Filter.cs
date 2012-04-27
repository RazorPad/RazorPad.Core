using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RazorPad.ViewModels
{
	internal class Filter<T> 
		where T : ISearchable
	{
		// main filer text
		private string _text;

		/// <summary>
		/// Indicates if the filter is empty.
		/// </summary>
		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(_text); }
		}

		/// <summary>
		/// Parse the filter text. Can be used to performa advanced operations such as regex search
		/// </summary>
		public void Parse(string text)
		{
			// store main text
			_text = text.Trim().ToLower();

			// enhancement: add regex filtering support				
		}

		/// <summary>
		/// Return true if the Reference matches the filter.
		/// </summary>
		public bool Matches(T item)
		{
			// check name
			return item.Name.ToLower().Contains(_text);
		}
	}
}
