module.exports = function (callback,data, output) {
    const PDFMerge = require('pdf-merge');

    PDFMerge(files)
        .then((buffer) => {
            callback(null, buffer);
        });
};