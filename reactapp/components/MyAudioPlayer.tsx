'use client';

import * as React from 'react';
import Image from "next/image"
import AudioProgressBar from './AudioProgressBar';
import useSong from '@/hooks/useSong';
import { useEffect, useRef, useState } from 'react';
import { formatDurationDisplay, getCoverImageSrc, getURL } from '@/libs/helpers';
import { Loader2, Pause, PauseCircleIcon, Play, PlayCircleIcon, Repeat, Shuffle, SkipBack, SkipForward, Volume2, Volume2Icon, VolumeIcon, VolumeX, VolumeXIcon } from 'lucide-react';
import { Slider } from './ui/slider';
import { Button } from './ui/button';
import Link from 'next/link';
import { useMediaQuery } from '@custom-react-hooks/use-media-query';
import useAuth from '@/hooks/useAuth';
import AudioPlayer, { RHAP_UI } from 'react-h5-audio-player'
import 'react-h5-audio-player/lib/styles.css'
import { toast } from '@/hooks/use-toast';
import { useIsIOS } from '@/hooks/useIsIos';

export default function MyAudioPlayer() {
  const { currentSong, next, prev, play, isPlaying, queue } = useSong();
  const isDesktop = useMediaQuery("(min-width: 768px)");
  const { isAuthenticated } = useAuth();

  const audioRef = useRef<HTMLAudioElement | null>(null);

  const [isReady, setIsReady] = useState(false);
  const [duration, setDuration] = useState(0);
  const [buffered, setBuffered] = useState(0);
  const [volume, setVolume] = useState(isDesktop ? 0.2 : 1);
  const [showVolumeSlider, setShowVolumeSlider] = useState(false)
  const [currentTime, setCurrentTime] = useState(0)
  const [isSeeking, setIsSeeking] = useState(false);
  const isIOS = useIsIOS();
  const playerRef = useRef<AudioPlayer>(null)

  const [isMuted, setIsMuted] = useState(false)
  //const [isPlaying, setIsPlaying] = useState(false)

  // const durationDisplay = formatDurationDisplay(duration);
  // const elapsedDisplay = formatDurationDisplay(currentTime);

  // useEffect(() => {
  //   play(false);
  //   setCurrentTime(0);
  //   const audio = audioRef.current

  //   if (!audio || !currentSong?.song.songPublicId) return;

  //   const updateTime = () => setCurrentTime(audio.currentTime)
  //   const updateDuration = () => setDuration(audio.duration)

  //   const handleError = (e: ErrorEvent) => {
  //     toast({
  //       title: "Error",
  //       description: "Failed to load audio. Please try again.",
  //       variant: "destructive",
  //     })
  //   }

  //   audio.addEventListener('timeupdate', updateTime)
  //   audio.addEventListener('loadedmetadata', updateDuration)
  //   audio.addEventListener('ended', handleNext)
  //   audio.addEventListener('error', handleError)

  //   // Try to load the audio
  //   audio.load()

  //   const timeout = setTimeout(() => {
  //     play(true);
  //   }, 500);

  //   return () => {
  //     audio.removeEventListener('timeupdate', updateTime)
  //     audio.removeEventListener('loadedmetadata', updateDuration)
  //     audio.removeEventListener('ended', handleNext)
  //     audio.removeEventListener('error', handleError)
  //     clearTimeout(timeout);
  //   }
  // }, [currentSong?.song.songPublicId])

  const handleNext = () => {
    setCurrentTime(0);
    next();
  };

  // const handlePrev = () => {
  //   setCurrentTime(0);
  //   prev();
  // };

  // // const handleEnded = () => {
  // //   play(false);
  // //   handleNext();
  // // };

  // const togglePlayPause = () => {
  //   play(!isPlaying);
  // };

  useEffect(() => {
    const audio = playerRef.current?.audio.current
    if (!audio) return;
    if (isPlaying) {
      const playPromise = audio.play();
      if (playPromise !== undefined) {
        playPromise.then(_ => {
        })
          .catch(error => {
            play(false);
          });
      }
    } else {
      audio.pause()
    }
  }, [isPlaying]);

  // useEffect(() => {
  //   const volumeValue = isDesktop ? 0.2 : 1;
  //   setVolume(volumeValue);
  //   if (audioRef.current) {
  //     audioRef.current.volume = volumeValue;
  //   }
  // }, [isDesktop]);

  // const handleBufferProgress = (e: React.SyntheticEvent<HTMLAudioElement, Event>) => {
  //   const audio = e.currentTarget;
  //   if (audio.buffered.length > 0) {
  //     setBuffered(audio.buffered.end(audio.buffered.length - 1));
  //   }
  // };

  // const handleProgressChange = (values: number[]) => {
  //   const audio = audioRef.current;
  //   if (audio) {
  //     const newTime = values[0];
  //     setCurrentTime(newTime);
  //     if (!isIOS) {
  //       audio.currentTime = newTime;
  //     }
  //   }
  // };

  // const handleProgressChangeCommitted = () => {
  //   const audio = audioRef.current;
  //   if (audio && isIOS) {
  //     audio.currentTime = currentTime;
  //   }
  //   setIsSeeking(false);
  // };

  // const handleVolumeChange = (values: number[]) => {
  //   const volumeValue = values[0];
  //   if (!audioRef.current) return;
  //   audioRef.current.volume = volumeValue;
  //   setVolume(volumeValue);
  // };

  // const handleMuteUnmute = () => {
  //   if (!audioRef.current) return;

  //   if (audioRef.current.volume !== 0) {
  //     audioRef.current.volume = 0;
  //   } else {
  //     audioRef.current.volume = 0.2;
  //   }
  // };

  useEffect(() => {
    //play(false);
    const audio = playerRef.current?.audio.current
    if (!audio) return;

    // console.log(audio.duration);
    // setDuration(audio.duration);

    // if ('mediaSession' in navigator) {
    //   // Set up the Media Session metadata
    //   navigator.mediaSession.metadata = new MediaMetadata({
    //     title: currentSong.song.title,
    //     artist: currentSong.song.ownerName,
    //     album: "",
    //     artwork: [{ src: getCoverImageSrc(currentSong.song.albumPublicId) }],
    //   });

    //   // Set up action handlers
    //   navigator.mediaSession.setActionHandler('play', () => {
    //     //audio.play();
    //   });

    //   navigator.mediaSession.setActionHandler('pause', () => {
    //     //audio.pause();
    //   });

    //   navigator.mediaSession.setActionHandler('seekbackward', (details) => {
    //     const seekTime = audio.currentTime - (details.seekOffset || 10);
    //     audio.currentTime = Math.max(seekTime, 0);
    //   });

    //   navigator.mediaSession.setActionHandler('seekforward', (details) => {
    //     const seekTime = audio.currentTime + (details.seekOffset || 10);
    //     audio.currentTime = Math.min(seekTime, audio.duration);
    //   });

    //   navigator.mediaSession.setActionHandler('seekto', (details) => {
    //     if (details.fastSeek && 'fastSeek' in audio) {
    //       audio.fastSeek(details.seekTime!);
    //     } else {
    //       audio.currentTime = details.seekTime!;
    //     }
    //   });

    //   navigator.mediaSession.setActionHandler('stop', () => {
    //     //audio.pause();
    //     //audio.currentTime = 0;
    //   });
    // }

    const updateTime = () => setCurrentTime(audio.currentTime)
    const updateDuration = () => setDuration(audio.duration)

    audio.addEventListener('timeupdate', updateTime)
    audio.addEventListener('loadedmetadata', updateDuration)
    audio.addEventListener('durationchange', updateDuration)
    audio.addEventListener('ended', handleNext)
    // const timeout = setTimeout(() => {
    //   play(true);
    // }, 500);
    //audio.addEventListener('play', () => play(true))
    //audio.addEventListener('pause', () => play(false))

    return () => {
      audio.removeEventListener('timeupdate', updateTime)
      audio.removeEventListener('loadedmetadata', updateDuration)
      audio.removeEventListener('ended', handleNext)
      audio.addEventListener('ended', handleNext)
      //clearTimeout(timeout);
      //audio.removeEventListener('play', () => play(true))
      //audio.removeEventListener('pause', () => play(false))
    }
  }, [playerRef.current?.audio.current])

  const togglePlay = () => {
    play(!isPlaying);
  }

  const handleProgressChange = (newValue: number[]) => {
    if (playerRef.current?.audio.current) {
      playerRef.current.audio.current.currentTime = newValue[0]
      setCurrentTime(newValue[0])
    }
  }

  const handleVolumeChange = (newValue: number[]) => {
    const newVolume = newValue[0]
    setVolume(newVolume)
    setIsMuted(newVolume === 0)
    if (playerRef.current?.audio.current) {
      playerRef.current.audio.current.volume = newVolume / 100
    }
  }

  const toggleMute = () => {
    setIsMuted(!isMuted)
    setVolume(isMuted ? 100 : 0)
    if (playerRef.current?.audio.current) {
      playerRef.current.audio.current.volume = isMuted ? 1 : 0
    }
  }

  const formatTime = (time: number) => {
    const minutes = Math.floor(time / 60)
    const seconds = Math.floor(time % 60)
    return `${minutes}:${seconds.toString().padStart(2, '0')}`
  }

  return (<div className="fixed bottom-0 left-0 right-0 bg-background border-t border-border shadow-lg"><audio
    //ref={audioRef}
    preload="auto"
    controls
  >
    <source src={`${getURL()}api/songs/TEST/audio?audioType=audio/mpeg&contentLength=18914536`} type="audio/mpeg" />
  </audio></div>)

  if (!isAuthenticated || !queue.length || !currentSong?.song) {
    return <></>;
  }

  // return (
  //   <div className="fixed bottom-0 left-0 right-0 bg-background border-t border-border shadow-lg">
  //     <AudioPlayer
  //       src={`${getURL()}api/songs/${currentSong.song.songPublicId}/audio?audioType=${currentSong.contentType}&contentLength=${currentSong.song.contentLength}`}
  //       showSkipControls={true}
  //       showJumpControls={false}
  //       preload='none'
  //       layout="stacked-reverse"
  //       customProgressBarSection={[RHAP_UI.PROGRESS_BAR]}
  //       customControlsSection={[
  //         RHAP_UI.ADDITIONAL_CONTROLS,
  //         RHAP_UI.MAIN_CONTROLS,
  //         RHAP_UI.VOLUME_CONTROLS,
  //       ]}
  //       customAdditionalControls={[]}
  //       customVolumeControls={[
  //         <Slider
  //           key="volume-slider"
  //           className="w-[100px] hidden md:flex"
  //           defaultValue={[100]}
  //           max={100}
  //           step={1}
  //         />,
  //       ]}
  //       className="bg-transparent"
  //       autoPlayAfterSrcChange={true}
  //       onClickPrevious={handlePrev}
  //       onClickNext={handleNext}
  //       onEnded={handleNext}
  //       customIcons={{
  //         play: <Play className="w-6 h-6" />,
  //         pause: <Pause className="w-6 h-6" />,
  //         previous: <SkipBack className="w-6 h-6" />,
  //         next: <SkipForward className="w-6 h-6" />,
  //         volume: <VolumeIcon className="w-6 h-6 hidden md:block" />,
  //       }}
  //     >
  //       <div className="flex items-center space-x-4 px-4 py-2">
  //         <Image
  //           src={getCoverImageSrc(currentSong.song.albumPublicId)}
  //           alt={`${currentSong.song.title} cover art`}
  //           width={48}
  //           height={48}
  //           className="rounded-md"
  //         />
  //         <div className="flex-grow min-w-0">
  //           <Link href="#" className="text-foreground font-medium truncate block">
  //             {currentSong.song.title}
  //           </Link>
  //           <Link href={`/profiles/${currentSong?.song.ownerUsername}`} className="hover:underline text-muted-foreground text-sm truncate block">
  //             {currentSong?.song.ownerName}
  //           </Link>
  //         </div>
  //       </div>
  //     </AudioPlayer>
  //   </div>
  // )

  return (
    <div className="fixed bottom-0 inset-x-3 bg-background border-t border-border shadow-lg">
      <div className="max-w-screen-xl mx-auto">
        <Slider
          value={[currentTime]}
          max={duration || 100}
          step={1}
          className="w-full"
          onValueChange={handleProgressChange}
        />
        <div className="flex items-center justify-between px-4 py-2">
          <div className="flex items-center space-x-4">
            <Button variant="ghost" size="icon" onClick={() => playerRef.current?.audio.current && (playerRef.current.audio.current.currentTime -= 10)}>
              <SkipBack className="h-4 w-4" />
            </Button>
            <Button variant="ghost" size="icon" onClick={togglePlay}>
              {isPlaying ? <Pause className="h-4 w-4" /> : <Play className="h-4 w-4" />}
            </Button>
            <Button variant="ghost" size="icon" onClick={() => playerRef.current?.audio.current && (playerRef.current.audio.current.currentTime += 10)}>
              <SkipForward className="h-4 w-4" />
            </Button>
          </div>
          <div className="flex items-center space-x-4">
            {/* <img src={coverImage} alt={songTitle} className="w-10 h-10 rounded-md" />
            <div>
              <h3 className="font-semibold text-sm truncate">{songTitle}</h3>
              <Link href={artistUrl} className="text-xs text-muted-foreground hover:underline truncate block">
                {artistName}
              </Link>
            </div> */}
          </div>
          <div className="flex items-center space-x-2">
            <span className="text-xs text-muted-foreground">{formatTime(currentTime)}</span>
            <span className="text-xs text-muted-foreground">/</span>
            <span className="text-xs text-muted-foreground">{formatTime(duration)}</span>
            <Button variant="ghost" size="icon" onClick={toggleMute}>
              {isMuted ? <VolumeX className="h-4 w-4" /> : <Volume2 className="h-4 w-4" />}
            </Button>
            <Slider
              value={[volume]}
              max={100}
              step={1}
              className="w-24"
              onValueChange={handleVolumeChange}
            />
          </div>
        </div>
      </div>
      <AudioPlayer
        ref={playerRef}
        preload="metadata"
        src={`${getURL()}api/songs/T-K56/audio?audioType=audio/mpeg&contentLength=9247391`}
        //src={`${getURL()}api/songs/${currentSong.song.songPublicId}/audio?audioType=${currentSong.contentType}&contentLength=${currentSong.song.contentLength}`}
        autoPlay={false}
        style={{ display: 'none' }}
      />
    </div>
  );

  // return (
  //   <div className="sticky bottom-0 bg-background border-t pb-2 sm:pb-4 z-10">
  //     {/* <div className="h-1">
  //       <AudioProgressBar
  //         duration={duration}
  //         currentProgress={currentProgress}
  //         buffered={buffered}
  //         onValueChange={changeAudioProgress}
  //       />
  //     </div> */}
  //     <Slider
  //       value={[currentTime]}
  //       max={duration || 100}
  //       step={0.05}
  //       onValueChange={handleProgressChange}
  //       onValueCommit={handleProgressChangeCommitted}
  //       onPointerDown={() => setIsSeeking(true)}
  //       className="w-full"
  //     />
  //     {currentSong?.song.songPublicId && (
  //       <audio
  //         ref={audioRef}
  //         key={currentSong?.song.songPublicId}
  //         preload="metadata"
  //         onDurationChange={(e) => setDuration(e.currentTarget.duration)}
  //         onEnded={handleNext}
  //         onCanPlay={() => setIsReady(true)}
  //         onTimeUpdate={(e) => {
  //           if (!isSeeking) {
  //             setCurrentTime(e.currentTarget.currentTime);
  //           }
  //           handleBufferProgress(e);
  //         }}
  //         onProgress={handleBufferProgress}
  //       >
  //         <source
  //           src="/test/test.mp3"//{`${getURL()}api/songs/${currentSong.song.songPublicId}/audio?audioType=${currentSong.contentType}&contentLength=${currentSong.song.contentLength}`}
  //           //type={currentSong.contentType}
  //         />
  //       </audio>
  //     )}
  //     <div className="flex justify-between items-center mt-3 mb-3"
  //       onMouseEnter={() => setShowVolumeSlider(true)}
  //       onMouseLeave={() => setShowVolumeSlider(false)}>
  //       <div className="flex items-center gap-2 flex-1 mr-3">
  //         <Button onClick={handlePrev} variant="ghost" size="icon" className='rounded-full hover:bg-primary hover:text-primary-foreground transition-colors'>
  //           <SkipBack className="h-6 w-6" />
  //         </Button>
  //         <Button disabled={!isReady} variant="ghost" onClick={togglePlayPause} size="icon" className='rounded-full hover:bg-primary hover:text-primary-foreground transition-colors'>
  //           {!isReady && currentSong ? (
  //             <Loader2 className="h-10 w-10 animate-spin" />
  //           ) : isPlaying ? (
  //             <Pause className="h-10 w-10" />
  //           ) : (
  //             <Play className="h-10 w-10" />
  //           )}
  //         </Button>
  //         <Button onClick={handleNext} variant="ghost" size="icon" className='rounded-full hover:bg-primary hover:text-primary-foreground transition-colors'>
  //           <SkipForward className="h-6 w-6" />
  //         </Button>
  //         <span className="text-xs hidden sm:block">
  //           {elapsedDisplay}&nbsp;/&nbsp;{durationDisplay}
  //         </span>
  //       </div>
  //       <div className="flex items-center justify-center flex-1 max-w-[40%]">
  //         <div className="relative w-10 h-10 mr-2 flex-shrink-0">
  //           <Image
  //             src={getCoverImageSrc(currentSong.song.albumPublicId)}
  //             alt="Cover"
  //             layout="fill"
  //             objectFit="cover"
  //             className="rounded-md"
  //           />
  //         </div>
  //         <div className="flex flex-col overflow-hidden">
  //           <h3 className="font-medium text-sm truncate">{currentSong?.song.title ?? 'Track ID'}</h3>
  //           <p className="text-xs text-muted-foreground truncate">
  //             <Link href={`/profiles/${currentSong?.song.ownerUsername}`} className="hover:underline">
  //               {currentSong?.song.ownerName}
  //             </Link>
  //           </p>
  //         </div>
  //       </div>
  //       <div className="flex items-center justify-end space-x-2 flex-1">
  //         {isDesktop && showVolumeSlider && (
  //           <Slider
  //             defaultValue={[0.2]}
  //             min={0}
  //             max={1}
  //             step={0.01}
  //             value={[volume]}
  //             onValueChange={handleVolumeChange}
  //             className='max-w-28 hidden sm:inline-flex'
  //           />
  //         )}
  //         {isDesktop && (
  //           <Button onClick={handleMuteUnmute} variant="ghost" size="icon" className="hidden sm:inline-flex rounded-full hover:bg-primary hover:text-primary-foreground transition-colors">
  //             {volume === 0 ? (
  //               <VolumeX className="h-6 w-6" />
  //             ) : (
  //               <Volume2 className="h-6 w-6" />
  //             )}
  //           </Button>
  //         )}
  //       </div>
  //     </div>
  //   </div>
  // );
}
