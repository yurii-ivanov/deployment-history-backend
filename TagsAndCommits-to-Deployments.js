console.log('hi');

//give it the commits and tags and it will create a payload for POST deployments/many

const appName = 'ApplicationNameHere';
const commits = {
	"values": [
		{
			"id": "123",
			"authorTimestamp": 1648023918000,
		},
		{
			"id": "1234",
			"authorTimestamp": 1648023918001,
		}
	],
	"size": 2,
	"isLastPage": true,
	"start": 0,
	"limit": 50,
	"nextPageStart": 2
};
const tags = {
	"size": 2,
	"limit": 100,
	"isLastPage": true,
	"values": [
		{
			"latestCommit": "123",
		},
		{
			"latestCommit": "1234",
		}
	],
	"start": 0
};

tags.values.forEach((tag) => {
    const commit = commits.values.find((commit) => tag.latestCommit === commit.id);
    if(commit) {
        console.log(JSON.stringify({commitId: commit.id, timestamp: new Date(commit.authorTimestamp).toUTCString(), applicationName: appName})+',');
    }
});

