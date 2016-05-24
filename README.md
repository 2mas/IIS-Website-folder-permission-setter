# IIS-Website-folder-permission-setter
Looping through websites in IIS, giving the Application pool identity Modify-rights on the physical path of the site.
Make sure to set the Anonymous Authentication to Application pool identity on the server-node in IIS.

### Instructions
Add a reference to Microsoft.Web.Administration API / drop it in bin-folder to run and have IIS enabled.
