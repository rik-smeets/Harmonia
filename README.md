# Harmonia
Harmonia is a Windows desktop application, providing a user friendly way to extract audio from a YouTube video, convert it to an MP3, normalize the audio and set the MP3 tags. It supports multiple downloads at once.

## Features
- User friendly interface
- Portable desktop application (i.e. no installation necessary)
- Listens to your clipboard, no manual adding of YouTube URL's necessary
- Supports all YouTube videos
- Normalizes audio, so the volume is the same across all downloads
- Sets the file name and MP3 tags consistently
- Shows a toast message notifying your downloads are complete (if supported the version of Windows you're running)
- Supports multiple downloads at once (and when downloading, you can keep adding/starting new downloads)

## Download
Download the latest release of Harmonia over at [releases](https://github.com/rik-smeets/Harmonia/releases).

## Installation instructions
Harmonia is a portable application, meaning no installation is necessary. Simply run `Harmonia.exe` from the extracted ZIP-file. 

Note: for audio normalization, MP3Gain is used. You need to have MP3Gain installed, or you can manually set the location of the `mp3gain.exe` executable. Download and install MP3Gain from it's [official website](http://mp3gain.sourceforge.net/).
If MP3Gain is not installed/the executable cannot be located, audio normalization will be skipped. You will receive a notification beforehand.

## How to use Harmonia
### Instructions
1. Start Harmonia.
2. Harmonia is now monitoring your clipboard. Simply copy a valid YouTube URL (Ctrl + C).
3. The video will show up in the grid. Artist and title will be suggested. You can change these if you like.
4. Repeat for multiple videos if desired.
5. Start downloading and convertering by selecting *Start all*. 
6. Once downloads are activated, you can keep adding new YouTube videos. At any time, hit *Start all* again to also start downloading the new videos. Completed and running downloads will be ignored.

### Settings
List of settings:
- **Output directory**: set the directory where downloads will be stored. *Default: your Windows Music folder.*
- **MP3Gain executable location**: manually set the location of the `mp3gain.exe` executable. *Default: install path of MP3Gain.*
- **Theme and color scheme**: set the theme to either Dark or Light, and pick an accent color of your liking. *Default: Dark theme, accent color Olive.*

## Etymology
Harmonia is the Greek goddess of harmony and concord. Read more about her at [Wikipedia](https://en.wikipedia.org/wiki/Harmonia).

## Dependencies
Harmonia is built using the following dependencies:
- [Fody](https://github.com/Fody/Fody)
- [MahApps.Metro](https://github.com/MahApps/MahApps.Metro)
- [MSTest V2](https://github.com/microsoft/testfx)
- [Moq4](https://github.com/moq/moq4)
- [SharpClipboard](https://github.com/Willy-Kimura/SharpClipboard)
- [Shouldly](https://github.com/shouldly/shouldly)
- [Taglib](https://github.com/mono/taglib-sharp/)
- [Unity Container](https://github.com/unitycontainer/unity)
- [Xabe.FFmpeg](https://github.com/tomaszzmuda/Xabe.FFmpeg)
- [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode)