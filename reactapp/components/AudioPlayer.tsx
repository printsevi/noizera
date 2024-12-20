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

  const durationDisplay = formatDurationDisplay(duration);
  const elapsedDisplay = formatDurationDisplay(currentTime);

  useEffect(() => {
    play(false);
    setCurrentTime(0);
    const audio = audioRef.current

    if (!audio || !currentSong?.song.songPublicId) return;

    const updateTime = () => setCurrentTime(audio.currentTime)
    const updateDuration = () => setDuration(audio.duration)

    const handleError = (e: ErrorEvent) => {
      toast({
        title: "Error",
        description: "Failed to load audio. Please try again.",
        variant: "destructive",
      })
    }

    audio.addEventListener('timeupdate', updateTime)
    audio.addEventListener('loadedmetadata', updateDuration)
    audio.addEventListener('ended', handleNext)
    audio.addEventListener('error', handleError)

    // Try to load the audio
    audio.load()

    const timeout = setTimeout(() => {
      play(true);
    }, 500);

    return () => {
      audio.removeEventListener('timeupdate', updateTime)
      audio.removeEventListener('loadedmetadata', updateDuration)
      audio.removeEventListener('ended', handleNext)
      audio.removeEventListener('error', handleError)
      clearTimeout(timeout);
    }
  }, [currentSong?.song.songPublicId])

  const handleNext = () => {
    setCurrentTime(0);
    next();
  };

  const handlePrev = () => {
    setCurrentTime(0);
    prev();
  };

  // const handleEnded = () => {
  //   play(false);
  //   handleNext();
  // };

  const togglePlayPause = () => {
    play(!isPlaying);
  };

  useEffect(() => {
    if (isPlaying) {
      const playPromise = audioRef.current?.play();
      if (playPromise !== undefined) {
        playPromise.then(_ => {
        })
          .catch(error => {
            play(false);
          });
      }
    } else {
      audioRef.current?.pause();
    }
  }, [isPlaying]);

  useEffect(() => {
    const volumeValue = isDesktop ? 0.2 : 1;
    setVolume(volumeValue);
    if (audioRef.current) {
      audioRef.current.volume = volumeValue;
    }
  }, [isDesktop]);

  const handleBufferProgress = (e: React.SyntheticEvent<HTMLAudioElement, Event>) => {
    const audio = e.currentTarget;
    if (audio.buffered.length > 0) {
      setBuffered(audio.buffered.end(audio.buffered.length - 1));
    }
  };

  const handleProgressChange = (values: number[]) => {
    const audio = audioRef.current;
    if (audio) {
      const newTime = values[0];
      setCurrentTime(newTime);
      if (!isIOS) {
        audio.currentTime = newTime;
      }
    }
  };

  const handleProgressChangeCommitted = () => {
    const audio = audioRef.current;
    if (audio && isIOS) {
      audio.currentTime = currentTime;
    }
    setIsSeeking(false);
  };

  const handleVolumeChange = (values: number[]) => {
    const volumeValue = values[0];
    if (!audioRef.current) return;
    audioRef.current.volume = volumeValue;
    setVolume(volumeValue);
  };

  const handleMuteUnmute = () => {
    if (!audioRef.current) return;

    if (audioRef.current.volume !== 0) {
      audioRef.current.volume = 0;
    } else {
      audioRef.current.volume = 0.2;
    }
  };

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
    <div className="sticky bottom-0 bg-background border-t pb-2 sm:pb-4 z-10">
      {/* <div className="h-1">
        <AudioProgressBar
          duration={duration}
          currentProgress={currentProgress}
          buffered={buffered}
          onValueChange={changeAudioProgress}
        />
      </div> */}
      <Slider
        value={[currentTime]}
        max={duration || 100}
        step={0.05}
        onValueChange={handleProgressChange}
        onValueCommit={handleProgressChangeCommitted}
        onPointerDown={() => setIsSeeking(true)}
        className="w-full"
      />
      {currentSong?.song.songPublicId && (
        <audio
          ref={audioRef}
          key={currentSong?.song.songPublicId}
          //preload="metadata"
          onDurationChange={(e) => setDuration(e.currentTarget.duration)}
          onEnded={handleNext}
          onCanPlay={() => setIsReady(true)}
          onTimeUpdate={(e) => {
            if (!isSeeking) {
              setCurrentTime(e.currentTarget.currentTime);
            }
            handleBufferProgress(e);
          }}
          onProgress={handleBufferProgress}
        >
          <source
            src={`${getURL()}api/songs/${currentSong.song.songPublicId}/audio?audioType=${currentSong.contentType}&contentLength=${currentSong.song.contentLength}`}
            type={currentSong.contentType}
          />
        </audio>
      )}
      <div className="flex justify-between items-center mt-3 mb-3"
        onMouseEnter={() => setShowVolumeSlider(true)}
        onMouseLeave={() => setShowVolumeSlider(false)}>
        <div className="flex items-center gap-2 flex-1 mr-3">
          <Button onClick={handlePrev} variant="ghost" size="icon" className='rounded-full hover:bg-primary hover:text-primary-foreground transition-colors'>
            <SkipBack className="h-6 w-6" />
          </Button>
          <Button disabled={!isReady} variant="ghost" onClick={togglePlayPause} size="icon" className='rounded-full hover:bg-primary hover:text-primary-foreground transition-colors'>
            {!isReady && currentSong ? (
              <Loader2 className="h-10 w-10 animate-spin" />
            ) : isPlaying ? (
              <Pause className="h-10 w-10" />
            ) : (
              <Play className="h-10 w-10" />
            )}
          </Button>
          <Button onClick={handleNext} variant="ghost" size="icon" className='rounded-full hover:bg-primary hover:text-primary-foreground transition-colors'>
            <SkipForward className="h-6 w-6" />
          </Button>
          <span className="text-xs hidden sm:block">
            {elapsedDisplay}&nbsp;/&nbsp;{durationDisplay}
          </span>
        </div>
        <div className="flex items-center justify-center flex-1 max-w-[40%]">
          <div className="relative w-10 h-10 mr-2 flex-shrink-0">
            <Image
              src={getCoverImageSrc(currentSong.song.albumPublicId)}
              alt="Cover"
              layout="fill"
              objectFit="cover"
              className="rounded-md"
            />
          </div>
          <div className="flex flex-col overflow-hidden">
            <h3 className="font-medium text-sm truncate">{currentSong?.song.title ?? 'Track ID'}</h3>
            <p className="text-xs text-muted-foreground truncate">
              <Link href={`/profiles/${currentSong?.song.ownerUsername}`} className="hover:underline">
                {currentSong?.song.ownerName}
              </Link>
            </p>
          </div>
        </div>
        <div className="flex items-center justify-end space-x-2 flex-1">
          {isDesktop && showVolumeSlider && (
            <Slider
              defaultValue={[0.2]}
              min={0}
              max={1}
              step={0.01}
              value={[volume]}
              onValueChange={handleVolumeChange}
              className='max-w-28 hidden sm:inline-flex'
            />
          )}
          {isDesktop && (
            <Button onClick={handleMuteUnmute} variant="ghost" size="icon" className="hidden sm:inline-flex rounded-full hover:bg-primary hover:text-primary-foreground transition-colors">
              {volume === 0 ? (
                <VolumeX className="h-6 w-6" />
              ) : (
                <Volume2 className="h-6 w-6" />
              )}
            </Button>
          )}
        </div>
      </div>
    </div>
  );
}
