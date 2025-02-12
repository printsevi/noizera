"use client"

import { useState } from 'react'
import { Badge } from "@/components/ui/badge"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { ArrowUpDown } from 'lucide-react'
import useAxiosPrivate from '@/hooks/useAxiosPrivate'
import useAuth from '@/hooks/useAuth'
import useUser from '@/hooks/useUser'
import { useRouter } from 'next/navigation';
import useSWR from 'swr'
import { ProfileType } from '@/api/common'
import getMyAlbums, { GetMyAlbumsResponse } from '@/api/users/getMyAlbums'
import Link from 'next/link'
import getMySongs, { GetMySongsResponse } from '@/api/users/getMySongs'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'

export default function DashboardContent() {
  const { axiosPrivate, isReady } = useAxiosPrivate();
  const { isAuthenticated, auth } = useAuth();
  const { user, setUser } = useUser();
  const router = useRouter();
  const { data, isLoading } = useSWR(isReady && isAuthenticated && (user?.profileType === ProfileType.Artist || user?.profileType === ProfileType.Label) ? getMyAlbums.name : null, () => getMyAlbums(axiosPrivate, auth.userId!), {
    revalidateIfStale: true,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });
  const { data: songsData, isLoading: songsIsLoading } = useSWR(isReady && isAuthenticated && (user?.profileType === ProfileType.Artist || user?.profileType === ProfileType.Label) ? getMySongs.name : null, () => getMySongs(axiosPrivate, auth.userId!), {
    revalidateIfStale: true,
    revalidateOnFocus: false,
    revalidateOnReconnect: false
  });
  const [sortColumn, setSortColumn] = useState<keyof GetMyAlbumsResponse>('title')
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc')
  const [sortSongsColumn, setSortSongsColumn] = useState<keyof GetMySongsResponse>('title')
  const [sortSongsDirection, setSortSongsDirection] = useState<'asc' | 'desc'>('asc')

  const sortedAlbums = data?.data ? [...data.data!].sort((a, b) => {
    if (a[sortColumn] < b[sortColumn]) return sortDirection === 'asc' ? -1 : 1
    if (a[sortColumn] > b[sortColumn]) return sortDirection === 'asc' ? 1 : -1
    return 0
  }) : [];

  const sortedSongs = songsData?.data ? [...songsData.data!].sort((a, b) => {
    if (a[sortSongsColumn] < b[sortSongsColumn]) return sortSongsDirection === 'asc' ? -1 : 1
    if (a[sortSongsColumn] > b[sortSongsColumn]) return sortSongsDirection === 'asc' ? 1 : -1
    return 0
  }) : [];

  const toggleSort = (column: keyof GetMyAlbumsResponse) => {
    if (column === sortColumn) {
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc')
    } else {
      setSortColumn(column)
      setSortDirection('asc')
    }
  }

  const toggleSongsSort = (column: keyof GetMySongsResponse) => {
    if (column === sortSongsColumn) {
      setSortSongsDirection(sortSongsDirection === 'asc' ? 'desc' : 'asc')
    } else {
      setSortSongsColumn(column)
      setSortSongsDirection('asc')
    }
  }

  return (
    <div className="container mx-auto py-10">
      <Tabs defaultValue="albums" className="w-full">
        <TabsList className={`grid w-full grid-cols-2`}>
          <TabsTrigger value="albums">Your Albums</TabsTrigger>
          <TabsTrigger value="songs">Your Songs</TabsTrigger>
        </TabsList>
        <TabsContent value="albums" className="mt-6">
          <div className="rounded-md border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead onClick={() => toggleSort('publicId')} className="cursor-pointer">
                    Link <ArrowUpDown className="ml-2 h-4 w-4 inline" />
                  </TableHead>
                  <TableHead onClick={() => toggleSort('title')} className="cursor-pointer">
                    Title <ArrowUpDown className="ml-2 h-4 w-4 inline" />
                  </TableHead>
                  <TableHead onClick={() => toggleSort('releaseDate')} className="cursor-pointer">
                    Release Date <ArrowUpDown className="ml-2 h-4 w-4 inline" />
                  </TableHead>
                  <TableHead onClick={() => toggleSort('status')} className="cursor-pointer">
                    Status <ArrowUpDown className="ml-2 h-4 w-4 inline" />
                  </TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {sortedAlbums.map((album) => (
                  <TableRow key={album.publicId}>
                    <TableCell>
                      {album.status === "Released" ? <Link href={`/collections/${album.publicId.toLowerCase()}`} className="hover:underline">{album.publicId}</Link> : ""}
                    </TableCell>
                    <TableCell className='truncate max-w-[100px]'>{album.title}</TableCell>
                    <TableCell>{album.releaseDate}</TableCell>
                    <TableCell>
                      <Badge>
                        {album.status === "Submitted" ? "Processing" : album.status}
                      </Badge>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </TabsContent>
        <TabsContent value="songs" className="mt-6">
          <div className="rounded-md border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead onClick={() => toggleSongsSort('title')} className="cursor-pointer">
                    Title <ArrowUpDown className="ml-2 h-4 w-4 inline" />
                  </TableHead>
                  <TableHead onClick={() => toggleSongsSort('streamCount')} className="cursor-pointer">
                    Streams <ArrowUpDown className="ml-2 h-4 w-4 inline" />
                  </TableHead>
                  <TableHead onClick={() => toggleSongsSort('likeCount')} className="cursor-pointer">
                    Likes <ArrowUpDown className="ml-2 h-4 w-4 inline" />
                  </TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {sortedSongs.map((song) => (
                  <TableRow key={song.publicId}>
                    <TableCell className='truncate max-w-[100px]'>{song.title}</TableCell>
                    <TableCell>{song.streamCount}</TableCell>
                    <TableCell>{song.likeCount}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </TabsContent>
      </Tabs>

    </div>
  )
}