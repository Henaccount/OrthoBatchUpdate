# OrthoBatchUpdate (Plant 3D 2025)
OrthoBatchUpdate - Example Code - Use at own risk!

<li>compile with Visual Studio</li>
<li>load dll with "netload" from commandline</li>
<li>run "OrthoBatchUpdateALL" or "OrthoBatchUpdateOnlyModified" command from commandline</li>

<li>the script opens all ortho drawings from the project (project manager) and updates all ortho views</li>
<li>it is using the "PLANTORTHOSILENTUPDATE" system variable with value "1" and sets it back to "0" after script execution</li>
<li>it is using the "PLANTORTHOUPDATE" command with the (new since 2025) option "ALL"</li>

<li>"OrthoBatchUpdateOnlyModified" checks the timestamps of the related 3d files for a ortho drawing, if one timestamp is newer than the ortho, then update will be done on all views of that ortho</li>
<li>note that there can be several scenarios where "OrthoBatchUpdateOnlyModified" can not detect the actual update situation, e.g. related 3d drawings did change, but somebody modified the ortho later without updating the views, so you should consider this</li>
<li>"OrthoBatchUpdateALL" is always the safer variant</li>


