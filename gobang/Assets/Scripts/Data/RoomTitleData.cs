using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public static class RoomTitleData
{
    public static Dictionary<int, ClassManager.RoomTitle> roomTitles = new Dictionary<int, ClassManager.RoomTitle>();


    public static bool RoomTitles_Exist(int roomID)
    {
        if (roomTitles.ContainsKey(roomID))
            return true;
        return false;

    }

    public static void Add(int roomID, ClassManager.RoomTitle newRoom) {
        roomTitles.Add(roomID, newRoom);

        
    }

    public static void Update_Room(string _roomTitle) {
        ClassManager.RoomTitle roomTitle = JsonConvert.DeserializeObject<ClassManager.RoomTitle>(_roomTitle);

        int room_id = roomTitle.roomID;

        if (roomTitles.ContainsKey(room_id))
        {
            if (roomTitle.player1Name == null && roomTitle.player2Name == null)
            {
                roomTitles.Remove(room_id);
            }
            else
            {
                roomTitles[room_id] = roomTitle;
            }
        }
        else
        {
            roomTitles.Add(room_id, roomTitle);
        }


    }


}
