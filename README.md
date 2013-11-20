PreCommitNoTabs
===============

Console executable for SVN precommit hook that will check for tabs in the change set and reject the commit if any file has tabs.

Credit goes to @devshorts (https://github.com/devshorts) for reviewing and suggestions.

How to Use
===============

- Create a script to call the precommit executeable.

	**PreCommitHook.cmd**

		/path/to/PreCommitNoTabs.exe %1 %2 ".cs;.cpp"
		
Where %1 is the repo path and %2 is the transactionId, this is passed in by SVN.

The last argument is the list of extensions for files we want to check.
	
- Add the script to the repos' hooks folder
	
