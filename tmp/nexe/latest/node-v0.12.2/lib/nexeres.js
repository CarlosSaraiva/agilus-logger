var embeddedFiles = {

};

module.exports.keys = function () { return Object.keys(embeddedFiles); }

module.exports.get = function (key) {
  if (embeddedFiles.hasOwnProperty(key)) {
    return new Buffer(embeddedFiles[key], 'base64');
  }
  else {
    //file was not embedded, throw err.
    throw new Error('Embedded file not found');
  }
}