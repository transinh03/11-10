import * as signalR from "@microsoft/signalr";

const hubUrl = import.meta.env.VITE_HUB_URL;

const connection = new signalR.HubConnectionBuilder()
  .withUrl(hubUrl)
  .withAutomaticReconnect()
  .build();

export default connection;
