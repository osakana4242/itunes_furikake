using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using iTunesLib;

using jp.osakana4242.core.LogOperator;

namespace jp.osakana4242.itunes_furikake
{
    public class TrackFieldAccessor
    {
        public static readonly TrackFieldAccessor[] items = new TrackFieldAccessor[] {
            TrackFieldAccessor.Create(
                name: "Album",
                getField: (_track) => _track.Album,
                setField: (_track, _value) => _track.Album = _value,
                getSortField: (_track) => _track.SortAlbum,
                setSortField: (_track, _value) => _track.SortAlbum = _value
            ),
            TrackFieldAccessor.Create(
                name: "AlbumArtist",
                getField: (_track) => _track.AlbumArtist,
                setField: (_track, _value) => _track.AlbumArtist = _value,
                getSortField: (_track) => _track.SortAlbumArtist,
                setSortField: (_track, _value) => _track.SortAlbumArtist = _value
            ),
            TrackFieldAccessor.Create(
                name: "Artist",
                getField: (_track) => _track.Artist,
                setField: (_track, _value) => _track.Artist = _value,
                getSortField: (_track) => _track.SortArtist,
                setSortField: (_track, _value) => _track.SortArtist = _value
            ),
            TrackFieldAccessor.Create(
                name: "Composer",
                getField: (_track) => _track.Composer,
                setField: (_track, _value) => _track.Composer = _value,
                getSortField: (_track) => _track.SortComposer,
                setSortField: (_track, _value) => _track.SortComposer = _value
            ),
            TrackFieldAccessor.Create(
                name: "Name",
                getField: (_track) => _track.Name,
                setField: (_track, _value) => _track.Name = _value,
                getSortField: (_track) => _track.SortName,
                setSortField: (_track, _value) => _track.SortName = _value
            ),
            TrackFieldAccessor.Create(
                name: "Show",
                getField: (_track) => _track.Show,
                setField: (_track, _value) => _track.Show = _value,
                getSortField: (_track) => _track.SortShow,
                setSortField: (_track, _value) => _track.SortShow = _value
            ),
        };

        public string name;
        public System.Func<IITFileOrCDTrack, string> getField;
        public System.Action<IITFileOrCDTrack, string> setField;
        public System.Func<IITFileOrCDTrack, string> getSortField;
        public System.Action<IITFileOrCDTrack, string> setSortField;

        public static TrackFieldAccessor Create(
        string name,
        System.Func<IITFileOrCDTrack, string> getField,
        System.Action<IITFileOrCDTrack, string> setField,
        System.Func<IITFileOrCDTrack, string> getSortField,
        System.Action<IITFileOrCDTrack, string> setSortField
            )
        {
            return new TrackFieldAccessor()
            {
                name = name,
                getField = _track => getField(_track) ?? "",
                setField = setField,
                getSortField = _track => getSortField(_track) ?? "",
                setSortField = setSortField,
            };
        }
    }
}
