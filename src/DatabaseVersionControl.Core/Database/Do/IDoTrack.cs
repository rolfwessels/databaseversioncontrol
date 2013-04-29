

using System;	
namespace Intercontinental.Core.Database.Do {	
	public interface IDoTrack {
		 String DatabaseName { get; set;}
         double Version { get; set; } 
		 DateTime CreateDate { get; set;} 
		 DateTime UpdateDate { get; set;} 
	
	}
}
