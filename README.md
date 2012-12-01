# WavStagno
WavStagno is a C#.Net (VS2012) project to implement [Steganography](http://en.wikipedia.org/wiki/Steganography) by hiding a secret text message in Audio file (Wave format) and extracting it back.

## WavStagno - The Approach
Several Steganography algorithms exist that can be used to hide data within existing file and then extracting it back, all of them in the end follow same method, tinkering with Byte streams of both, the target file and the message to be hidden, we've done the same.

### Structure of Wave Audio file.
Following is the standard structure of WAVE Audio format, from which `WavAudio` object is constructed and left and right audio channel streams are extracted to hide data.

![ScreenShot](https://raw.github.com/kushalpandya/WavStagno/master/Documents/WAVE%20Structure.png)

### Hiding the Message into Audio Stream Channels.
The `StagnoHelper` class uses `WaveAudio` object and stores message stream (using its `HideMessage` method) into its channel streams starting from the last bit of every 8 bit block of the entire audio channel stream (both left and right), where the first element of the stream contains the quotient and remainder of the message length after dividing it with 65535 (upper limit of `short` data-type), and remaining elements contain actual message data. Refer to `HideMessage` method of [StagnoHelper](https://github.com/kushalpandya/WavStagno/blob/master/WavStagno/StagnoHelper.cs) class.

### Extract the Message from Audio Stream Channels.
The channel streams extracted from `WaveAudio` object are then traversed in `ExtractMessage` method of `StagnoHelper` and whole hiding process reversed and original message is recovered. Refer to `ExtractMessage` method of [StagnoHelper](https://github.com/kushalpandya/WavStagno/blob/master/WavStagno/StagnoHelper.cs) class.


## Platforms, Tools and Technologies used

* **IDE** - Microsoft Visual Studio 2010/2012.
* **Language** - Visual C#.Net.
* **Development OS** - Windows 7/8.

## Namespace Structure

* `WavStagno` - Main Namespace.
* `WavStagno.Media` - Audio Handling Classes.

## Developers

* [Kushal Pandya](https://github.com/kushalpandya)
* [Meherzad Lahewala](https://github.com/meherzad)