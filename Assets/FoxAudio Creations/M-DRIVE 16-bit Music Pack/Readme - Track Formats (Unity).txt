F O X A U D I O   C R E A T I O N S   T R A C K   F O R M A T S

FULL TRACK
The whole track - intro, main body, maybe an extra loop, ending. A better fit for non-looping playback and
suitable for listening. Ogg Vorbis version includes loop comments, so in RPG Maker and any other engine
made compatible, will play intro once and then loop seamlessly.

SINGLE LOOP
Designed to loop as seamlessly as possible in any environment, with no intro or ending. Minimal length and
thus file size, but will probably end too abruptly for non-looping playback.

Please note: there is a very brief moment of silence at the start of each Single Loop MP3, which is only
just audible when the music loops. This is added during my software's MP3 exporting process and I'm afraid
there is nothing I can do to remove it. If your engine supports the file format, I STRONGLY recommend
using the Ogg Vorbis Single Loop tracks.

INTRO-LOOP
Ogg Vorbis only. Like Full Track, will play the intro once and then loop seamlessly in any engine made
compatible with LOOPSTART and LOOPLENGTH comments, such as RPG Maker, but only includes one loop and no
ending, to save on file size. My recommended format for game engines, if the loop comments are supported.
Will probably end too abruptly for non-looping playback or listening.

ONE-SHOT
Designed purely to be played once, and is not intended for looping. Usually reserved for jingles, though
some longer tracks come with this format, too. Suitable for listening.



ABOUT M4A FILES AND UNITY
Please note that M-DRIVE does not include M4A files when bought from the Unity Asset store, as Unity does
not normally support them. However, legitimate buyers may email me and request them.







About Loop Comments

Ogg Vorbis format Full Track and Intro-Loop files come embedded with LOOPSTART and LOOPLENGTH comments. In
RPG Maker, these tracks will play an intro and then loop seamlessly from a predefined point. I have seen
this exact format being used in some Japanese games, too, and these files might be compatible with some
other game engines.

If you want to make your game's chosen engine work with these loop comments, I'm afraid I'm not a
programmer, so I can't provide specific instructions. But, should you wish to figure out how to do it for
yourself, here's a rough overview of how it works:


- For those who don't know, audio files are composed of "samples." These are to audio much as frames are
  to an animation or video file.

- So, from a programming perspective, the most accurate measure of time you can work with in audio is
  the number of samples that have passed.

- Sample rate can vary from file to file, much like frame rate in video files. All of M-DRIVE's music
  is encoded at 48,000 Hz. So that's 48,000 samples each second.

- Each of my Ogg Vorbis files (except for Single Loop and One-Shot files) contains a set of comments:
  LOOPSTART and LOOPLENGTH, and a value for each of these. That value is how many samples the respective
  comment is trying to tell the engine to work with.

- As their names very heavily suggest, LOOPSTART is the point at which the loop begins, and LOOPLENGTH is
  how many samples long it is, measured from the LOOPSTART comment onwards.

- So, at 48,000 Hz, if a file's LOOPSTART comment has a value of 24000 and its LOOPLENGTH comment has a
  value of 480,000, by design, it will play for ten and a half seconds, then return to the half-second
  mark to continue playing endlessly.

- If you can program your game to look for these comments, to start looping at <LOOPSTART> samples and
  return to that point after <LOOPLENGTH> more samples, looping indefinitely, it will play back these
  files as intended.


If you want to view these loop comments for yourself, the simplest way is with Audacity, an open-source
audio editing tool. Just open the Ogg file with Audacity and go to Edit > Metadata... to see them.







©FoxAudio Creations 2018

For more free and commercially-licensed music and sound effects, check out foxaudiocreations.co.uk

Want to contact the composer? Reach me at TobySFWilkinson@gmail.com