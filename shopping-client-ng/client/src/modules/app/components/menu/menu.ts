import { Component } from "@angular/core";
import { WindowRef } from "../../services/windowRef";

@Component({
  selector: "app-menu",
  templateUrl: "menu.html",
  styleUrls: ["menu.scss"]
})
export class MenuComponent {
  private readonly _bodyCssClass = "show-menu";

  constructor(private _windowRef: WindowRef) {}
}
