const exec = require('child_process').exec;
const fs = require('fs');
const path = require('path');

const base_path = 'dist/starter-client/browser'
const files = getFilesFromPath(`./${base_path}`, '.css');
let data = [];

if (!files && files.length <= 0) {
	console.log("cannot find style files to purge");
	return;
}

for (let f of files) {
	const originalSize = getFilesizeInKiloBytes(`./${base_path}/${f}`) + "kb";
	var o = { "file": f, "originalSize": originalSize, "newSize": "" };
	data.push(o);
}

console.log("Run PurgeCSS...");

exec(`purgecss -css ${base_path}/*.css --content ${base_path}/index.html ${base_path}/*.js -o ${base_path}/`, function(error, stdout, stderr) {
	console.log("PurgeCSS done");
	console.log();

	for (let d of data) {
		const newSize = getFilesizeInKiloBytes(`./${base_path}/${d.file}`) + "kb";
		d.newSize = newSize;
	}

	console.table(data);
});

function getFilesizeInKiloBytes(filename) {
	var stats = fs.statSync(filename);
	var fileSizeInBytes = stats.size / 1024;
	return fileSizeInBytes.toFixed(2);
}

function getFilesFromPath(dir, extension) {
	let files = fs.readdirSync(dir);
	return files.filter(e => path.extname(e).toLowerCase() === extension);
}
