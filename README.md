PreCommitNoTabs
===============

Console executable for SVN precommit hook that will check for tabs in the change set and reject the commit if any file has tabs.

Credit goes to @devshorts (https://github.com/devshorts) for reviewing and suggestions.

How to Use
===============

- Create a script to call the precommit executeable.

	**PreCommitHook.cmd**

		/path/to/PreCommitNoTabs.exe
	
- Add the script to the repos' hooks folder
	
