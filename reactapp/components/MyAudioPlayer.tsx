'use client';

import * as React from 'react';
import Image from "next/image"
import useSong from '@/hooks/useSong';
import { useEffect, useRef, useState } from 'react';
import { formatDurationDisplay, getCoverImageSrc, getURL } from '@/libs/helpers';
import { Loader2, Pause, Play, SkipBack, SkipForward, Volume2, VolumeX } from 'lucide-react';
import { Slider } from './ui/slider';
import { Button } from './ui/button';
import Link from 'next/link';
import { useMediaQuery } from '@custom-react-hooks/use-media-query';
import useAuth from '@/hooks/useAuth';
import 'react-h5-audio-player/lib/styles.css'
import { useIsIOS } from '@/hooks/useIsIos';

export default function MyAudioPlayer() {
  const { currentSong, next, prev, play, isPlaying, queue } = useSong();
  const [audioContext, setAudioContext] = useState<AudioContext | null>(null);
  const isDesktop = useMediaQuery("(min-width: 768px)");
  const { isAuthenticated } = useAuth();

  const audioRef = useRef<HTMLAudioElement | null>(null);

  const [isReady, setIsReady] = useState(false);
  const [duration, setDuration] = useState(0);
  const [volume, setVolume] = useState(isDesktop ? 0.2 : 1);
  const [showVolumeSlider, setShowVolumeSlider] = useState(false)
  const [currentTime, setCurrentTime] = useState(0)
  const [isSeeking, setIsSeeking] = useState(false)

  const durationDisplay = formatDurationDisplay(duration);
  const elapsedDisplay = formatDurationDisplay(currentTime);

  useEffect(() => {
    if (!audioContext) {
      setAudioContext(new (window.AudioContext || (window as any).webkitAudioContext)());
    }
  }, [audioContext]);

  // Connect AudioContext to audio element
  useEffect(() => {
    const audio = audioRef.current;
    if (audio && audioContext) {
      const track = audioContext.createMediaElementSource(audio);
      track.connect(audioContext.destination);
    }
  }, [audioContext]);

  // Resume AudioContext on user interaction
  useEffect(() => {
    const handleInteraction = () => {
      if (audioContext?.state === 'suspended') {
        audioContext.resume();
      }
    };

    document.addEventListener('click', handleInteraction);
    document.addEventListener('touchstart', handleInteraction);

    return () => {
      document.removeEventListener('click', handleInteraction);
      document.removeEventListener('touchstart', handleInteraction);
    };
  }, [audioContext]);

  const handleNext = () => {
    play(false);
    const audio = audioRef.current;
    if (!audio) return;
    next();
    setCurrentTime(0);
    audio.currentTime = 0;
    play(true);
  };

  const handlePrev = () => {
    play(false);
    const audio = audioRef.current;
    if (!audio) return;
    prev();
    setCurrentTime(0);
    audio.currentTime = 0;
    play(true);
  };

  const togglePlayPause = () => {
    play(!isPlaying);
  };

  useEffect(() => {
    const audio = audioRef.current;
    if (!audio) return;
    if (currentSong?.presignedUrl) {
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
        if (!audio.paused) {
          audio.pause()
        }
      }
    }

  }, [isPlaying, currentSong?.presignedUrl]);

  useEffect(() => {
    const volumeValue = isDesktop ? 0.2 : 1;
    setVolume(volumeValue);
    if (audioRef.current) {
      audioRef.current.volume = volumeValue;
    }
  }, [isDesktop]);

  const handleProgressChange = (values: number[]) => {
    setIsSeeking(true);
    setCurrentTime(values[0]);
  };

  const handleTimeChange = (values: number[]) => {
    if (!isSeeking) {
      setCurrentTime(values[0]);
    }
  };

  const handleProgressCommit = (values: number[]) => {
    const audio = audioRef.current;
    if (audio) {
      const newTime = values[0];
      audio.currentTime = newTime;
    }
    setIsSeeking(false);
  };

  const handleVolumeChange = (values: number[]) => {
    const audio = audioRef.current;
    if (!audio) return;
    const volumeValue = values[0];
    audio.volume = volumeValue;
    setVolume(volumeValue);
  };

  const handleMuteUnmute = () => {
    if (!audioRef.current) return;
    if (audioRef.current.volume !== 0) {
      audioRef.current.volume = 0;
      setVolume(0);
    } else {
      audioRef.current.volume = 0.2;
      setVolume(0.2);
    }
  };

  useEffect(() => {
    if ('mediaSession' in navigator && currentSong?.song.songPublicId) {
      navigator.mediaSession.metadata = new MediaMetadata({
        title: currentSong.song.title,
        artist: currentSong.song.ownerName,
        artwork: [{ src: getCoverImageSrc(currentSong.song.albumPublicId), sizes: '512x512', type: 'image/jpeg' }]
      });

      navigator.mediaSession.setActionHandler('play', () => {
        play(true);
      });
      navigator.mediaSession.setActionHandler('pause', () => {
        play(false);
      });
      navigator.mediaSession.setActionHandler('nexttrack', () => {
        handleNext();
      });
      navigator.mediaSession.setActionHandler('previoustrack', () => {
        handlePrev();
      });
      navigator.mediaSession.setActionHandler('seekto', (details) => {
        if (details.seekTime !== undefined) {
          handleProgressCommit([details.seekTime]);
        }
      });
    }
  }, [currentSong?.song.songPublicId, play, handleNext, prev, handleProgressChange]);

  if (!isAuthenticated || !queue.length || !currentSong?.song) {
    return <></>;
  }

  return (
    <div className="sm:fixed sm:left-0 sm:right-3 ml-5 sm:ml-0 mr-5 sm:mr-0 bottom-0 bg-background pb-safe z-10">
      <Slider
        value={[currentTime]}
        max={duration || 100}
        step={0.01}
        onValueChange={handleProgressChange}
        onValueCommit={handleProgressCommit}
        className="w-full pt-3 sm:pt-0"
      />
      {currentSong.presignedUrl && (
        <audio
          ref={audioRef}
          key={currentSong.song.songPublicId}
          preload='metadata'
          onDurationChange={(e) => setDuration(e.currentTarget.duration)}
          onEnded={handleNext}
          onCanPlay={() => setIsReady(true)}
          onTimeUpdate={(e) => {
            handleTimeChange([e.currentTarget.currentTime]);
          }}
        >
          <source
            src={currentSong.presignedUrl}
            type={currentSong.contentType}
          />
        </audio>
      )}
      <div className="flex justify-between items-center mt-3 mb-3"
        onMouseEnter={() => setShowVolumeSlider(true)}
        onMouseLeave={() => setShowVolumeSlider(false)}>
        <div className="flex items-center gap-2 flex-1 mr-3">
          <Button onClick={handlePrev} variant="ghost" size="icon" className='rounded-full hover:bg-background'>
            <SkipBack className="h-6 w-6" />
          </Button>
          <Button disabled={!isReady} variant="ghost" onClick={togglePlayPause} size="icon" className='rounded-full hover:bg-background'>
            {!isReady && currentSong ? (
              <Loader2 className="h-10 w-10 animate-spin" />
            ) : isPlaying ? (
              <Pause className="h-10 w-10" />
            ) : (
              <Play className="h-10 w-10" />
            )}
          </Button>
          <Button onClick={handleNext} variant="ghost" size="icon" className='rounded-full hover:bg-background'>
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
              min={0}
              max={1}
              step={0.01}
              value={[volume]}
              onValueChange={handleVolumeChange}
              className='max-w-28 hidden sm:inline-flex'
            />
          )}
          {isDesktop && (
            <Button onClick={handleMuteUnmute} variant="ghost" size="icon" className="hidden sm:inline-flex rounded-full hover:bg-background">
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
